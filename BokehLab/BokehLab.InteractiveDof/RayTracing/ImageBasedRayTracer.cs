namespace BokehLab.InteractiveDof.RayTracing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using BokehLab.InteractiveDof;
    using BokehLab.InteractiveDof.DepthPeeling;
    using BokehLab.InteractiveDof.MultiViewAccum;
    using BokehLab.InteractiveDof.NeighborhoodBuffers;
    using BokehLab.Math;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    class ImageBasedRayTracer : IncrementalRenderer
    {
        static readonly string VertexShaderPath = "RayTracing/IbrtVS.glsl";
        static readonly string IbrtFragmentShaderPath = "RayTracing/IbrtFS.glsl";

        int vertexShader;
        int ibrtFragmentShader;
        int shaderProgram;

        public bool IncrementalModeEnabled { get; set; }

        int previewLensSampleTexture;
        int incrementalLensSampleTexture;
        //int pixelSampleTexture;

        int SqrtPreviewSampleCount { get; set; }
        int PreviewSampleCount { get { return SqrtPreviewSampleCount * SqrtPreviewSampleCount; } }

        int lensSampleTileSize = 32;

        Matrix4 sensorTransform;
        float[] sensorTransform3x3;

        public DepthPeeler DepthPeeler { get; set; }
        public NBuffers NBuffers { get; set; }

        public ImageBasedRayTracer()
        {
            SqrtPreviewSampleCount = 3;
            ViewsPerFrame = 1;
            //MaxIterations = SingleFrameIterations;
            IncrementalModeEnabled = true;
        }

        public void DrawSingleFrame(Scene scene, Navigation navigation)
        {
            DrawSingleFrame(0, scene, navigation);
        }

        protected override void DrawSingleFrame(int iteration, Scene scene, Navigation navigation)
        {
            Debug.Assert(DepthPeeler != null);

            Camera camera = navigation.Camera;

            ComputeSensorTransform(camera);

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // bind color and depth textures
            GL.ActiveTexture(TextureUnit.Texture0);
            int lensSampleTex = IncrementalModeEnabled
                ? incrementalLensSampleTexture
                : previewLensSampleTexture;
            GL.BindTexture(TextureTarget.Texture3D, lensSampleTex);
            //GL.ActiveTexture(TextureUnit.Texture1);
            //GL.BindTexture(TextureTarget.Texture1D, pixelSampleTexture);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2DArray, DepthPeeler.PackedDepthTextures);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2DArray, DepthPeeler.ColorTextures);
            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2DArray, NBuffers.NBuffersTextures);

            // enable IBRT shader
            GL.UseProgram(shaderProgram);

            // set shader parameters (textures, lens model, ...)
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "lensSamplesTexture"), 0);
            //GL.Uniform1(GL.GetUniformLocation(shaderProgram, "pixelSamplesTexture"), 1);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "packedDepthTexture"), 2);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "colorTexture"), 3);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "nBuffersTexture"), 4);

            GL.Uniform2(GL.GetUniformLocation(shaderProgram, "nBuffersSize"), NBuffers.Size);

            GL.Uniform2(GL.GetUniformLocation(shaderProgram, "sensorSize"), camera.SensorSize);
            //GL.Uniform1(GL.GetUniformLocation(shaderProgram, "sensorZ"), camera.SensorZ);
            GL.Uniform3(GL.GetUniformLocation(shaderProgram, "sensorShift"), camera.SensorShift3);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "near"), camera.Near);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "far"), camera.Far);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "lensFocalLength"), camera.Lens.FocalLength);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "lensApertureRadius"), camera.Lens.ApertureRadius);
            //Matrix4 perspective = camera.Perspective;
            //GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "perspective"), false, ref perspective);
            GL.Uniform4(GL.GetUniformLocation(shaderProgram, "frustumBounds"), camera.FrustumBounds);

            // jittering
            GL.Uniform2(GL.GetUniformLocation(shaderProgram, "screenSize"), new Vector2(Width, Height));
            GL.Uniform2(GL.GetUniformLocation(shaderProgram, "screenSizeInv"), new Vector2(1.0f / Width, 1.0f / Height));
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "sampleCount"), (IncrementalModeEnabled ? SampleCount : PreviewSampleCount));
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "sampleCountInv"), 1.0f / (IncrementalModeEnabled ? SampleCount : PreviewSampleCount));
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "sampleIndexOffset"), IncrementalModeEnabled ? SampleCount * iteration : 0);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "totalSampleCount"), IncrementalModeEnabled ? MaxTotalSampleCount : PreviewSampleCount);
            //GL.Uniform1(GL.GetUniformLocation(shaderProgram, "sampleCount"), SampleCount);
            //GL.Uniform1(GL.GetUniformLocation(shaderProgram, "sampleCountInv"), 1.0f / SampleCount);
            //GL.Uniform1(GL.GetUniformLocation(shaderProgram, "sampleIndexOffset"), SampleCount * iteration);
            //GL.Uniform1(GL.GetUniformLocation(shaderProgram, "totalSampleCount"), TotalSampleCount);

            GL.Uniform2(GL.GetUniformLocation(shaderProgram, "cameraShift"), camera.LensShift);
            GL.UniformMatrix3(GL.GetUniformLocation(shaderProgram, "sensorTransform"), 1, false, sensorTransform3x3);

            // draw the quad
            LayerHelper.DrawQuad();

            // disable shader
            GL.UseProgram(0);

            // unbind textures
            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void ComputeSensorTransform(Camera camera)
        {
            sensorTransform = Matrix4.Identity;
            if (Math.Abs(camera.SensorRotation.X) > 0)
            {
                sensorTransform *= Matrix4.CreateRotationX(camera.SensorRotation.X);
            }
            if (Math.Abs(camera.SensorRotation.Y) > 0)
            {
                sensorTransform *= Matrix4.CreateRotationY(camera.SensorRotation.Y);
            }
            sensorTransform3x3 = Matrix4x4To3x3Array(sensorTransform, ref sensorTransform3x3);
        }

        public override void Initialize(int width, int height)
        {
            base.Initialize(width, height);

            ShaderLoader.CreateSimpleShaderProgram(VertexShaderPath, IbrtFragmentShaderPath,
               out vertexShader, out ibrtFragmentShader, out shaderProgram);

            GL.Enable(EnableCap.Texture2D);

            RegenerateSampleTextures(lensSampleTileSize);
        }

        public override void Dispose()
        {
            if (shaderProgram != 0)
                GL.DeleteProgram(shaderProgram);
            if (vertexShader != 0)
                GL.DeleteShader(vertexShader);
            if (ibrtFragmentShader != 0)
                GL.DeleteShader(ibrtFragmentShader);

            if (previewLensSampleTexture != 0)
                GL.DeleteTexture(previewLensSampleTexture);

            if (incrementalLensSampleTexture != 0)
                GL.DeleteTexture(incrementalLensSampleTexture);

            //if (pixelSampleTexture != 0)
            //    GL.DeleteTexture(pixelSampleTexture);

            base.Dispose();
        }

        private void RegenerateSampleTextures(int tileSize)
        {
            if (incrementalLensSampleTexture == 0)
            {
                incrementalLensSampleTexture = GL.GenTexture();
            }
            if (previewLensSampleTexture == 0)
            {
                previewLensSampleTexture = GL.GenTexture();
            }
            //if (pixelSampleTexture == 0)
            //{
            //    pixelSampleTexture = GL.GenTexture();
            //}

            GenerateLensSamplesTextures(incrementalLensSampleTexture, MaxTotalSampleCount, tileSize);
            GenerateLensSamplesTextures(previewLensSampleTexture, PreviewSampleCount, tileSize);
            //GeneratePixelSamplesTexture(pixelSampleTexture, sqrtSampleCount, SampleCount);
        }

        private void GenerateLensSamplesTextures(int textureId, int totalSampleCount, int tileSize)
        {
            // size of a group of samples for a single pixel
            int bands = 2;
            int textureSize = bands * totalSampleCount * tileSize * tileSize;

            //IEnumerable<Vector2d> samples = GenerateLensSamples(tileSize, (int)Math.Sqrt(MaxTotalSampleCount)).GetEnumerator();

            int sqrtTotalSampleCount = (int)Math.Sqrt(totalSampleCount);
            Vector2[, ,] samples = new Vector2[tileSize, tileSize, totalSampleCount];
            Sampler sampler = new Sampler();

            for (int y = 0; y < tileSize; y++)
            {
                for (int x = 0; x < tileSize; x++)
                {
                    IEnumerable<Vector2> pixelSamples = sampler.CreateLensSamplesFloat(sqrtTotalSampleCount, ShuffleLensSamples);
                    int z = 0;
                    foreach (Vector2 sample in pixelSamples)
                    {
                        samples[x, y, z] = sample;
                        z++;
                    }
                }
            }

            GL.BindTexture(TextureTarget.Texture3D, textureId);

            IntPtr texturePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Half)) * textureSize);
            unsafe
            {
                int zStride = bands * tileSize * tileSize;
                for (int y = 0; y < tileSize; y++)
                {
                    for (int x = 0; x < tileSize; x++)
                    {
                        Half* row = (Half*)texturePtr + bands * (y * tileSize + x);
                        int index = 0;
                        // Z dimension
                        for (int sample = 0; sample < totalSampleCount; sample++)
                        {
                            Vector2 lensPos = samples[x, y, sample];
                            row[index] = (Half)lensPos.X;
                            row[index + 1] = (Half)lensPos.Y;
                            index += zStride;
                        }
                    }
                }
            }

            // TODO: could be an half float or unsigned byte instead of a float
            // TODO: two sample pair could be stored in one 4-channel value
            GL.TexImage3D(TextureTarget.Texture3D, 0, PixelInternalFormat.Rg16f,
                tileSize, tileSize, totalSampleCount, 0,
                PixelFormat.Rg, PixelType.HalfFloat, texturePtr);

            Marshal.FreeHGlobal(texturePtr);

            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Clamp);

        }

        //private IEnumerable<Vector2d> GenerateLensSamples(int tileSize, int sqrtSampleCount)
        //{
        //    int pixelCount = tileSize * tileSize;
        //    IEnumerator<Vector2d>[] jitteredSamplers = new IEnumerator<Vector2d>[pixelCount];
        //    Sampler sampler = new Sampler();
        //    for (int i = 0; i < pixelCount; i++)
        //    {
        //        jitteredSamplers[i] = sampler.GenerateJitteredSamples(MaxTotalSampleCount).GetEnumerator();
        //    }
        //    for (int sample = 0; sample < MaxTotalSampleCount; sample++)
        //    {
        //        for (int i = 0; i < pixelCount; i++)
        //        {
        //            jitteredSamplers[i].MoveNext();
        //            yield return jitteredSamplers[i].Current;
        //        }
        //    }
        //}

        private void GeneratePixelSamplesTexture(int textureId, int sqrtSampleCount, int sampleCount)
        {
            GL.BindTexture(TextureTarget.Texture1D, textureId);
            // size of a group of samples for a single pixel
            int bands = 2;
            int textureSize = bands * sampleCount;

            Sampler sampler = new Sampler();
            IntPtr texturePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Half)) * textureSize);
            unsafe
            {
                Half* row = (Half*)texturePtr;
                int index = 0;
                foreach (Vector2d sample in sampler.GenerateJitteredSamples(sqrtSampleCount))
                {
                    row[index] = (Half)(sample.X - 0.5f);
                    index++;
                    row[index] = (Half)(sample.Y - 0.5f);
                    index++;
                }
            }

            // TODO: could be an half float or unsigned byte instead of a float
            // TODO: two sample pair could be stored in one 4-channel value
            GL.TexImage1D(TextureTarget.Texture1D, 0, PixelInternalFormat.Rg16f,
                sampleCount, 0,
                PixelFormat.Rg, PixelType.HalfFloat, texturePtr);

            Marshal.FreeHGlobal(texturePtr);

            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        }

        private float[] Matrix4x4To3x3Array(Matrix4 matrix4x4, ref float[] matrix3x3)
        {
            if (matrix3x3 == null)
            {
                matrix3x3 = new float[9];
            }
            matrix3x3[0] = matrix4x4.M11;
            matrix3x3[1] = matrix4x4.M12;
            matrix3x3[2] = matrix4x4.M13;
            matrix3x3[3] = matrix4x4.M21;
            matrix3x3[4] = matrix4x4.M22;
            matrix3x3[5] = matrix4x4.M23;
            matrix3x3[6] = matrix4x4.M31;
            matrix3x3[7] = matrix4x4.M32;
            matrix3x3[8] = matrix4x4.M33;
            return matrix3x3;
        }

        public class IbrtPlayground
        {
            public static void TraceRay(Camera camera)
            {
                // pixel postion in the normalized sensor space [0;1]^2
                Vector2 texCoord = new Vector2(0.0f, 0.0f);

                // pixel corner in camera space
                // TODO: offset to pixel center or jitter the pixel area
                Vector3 pixelPos = new Vector3(
                    (0.5f - texCoord.X) * camera.SensorSize.X,
                    (0.5f - texCoord.Y) * camera.SensorSize.Y,
                    camera.SensorZ);

                //Vector3 colorSum = Vector3(0.0, 0.0, 0.0);
                //iVector2 steps = iVector2(3, 3);

                float apertureRadius = camera.Lens.ApertureRadius;
                //float apertureRadius = 0.0001f;

                //Vector2 offsetStep = (2.0 * apertureRadius) * Vector2(1.0 / Vector2(steps - iVector2(1, 1)));
                //for (int y = 0; y < steps.y; y++) {
                //for (int x = 0; x < steps.x; x++) {
                //Vector3 lensOffset = Vector3(
                //float(x) * offsetStep.x - apertureRadius,
                //float(y) * offsetStep.y - apertureRadius, 0.0);

                Vector3 lensOffset = new Vector3(apertureRadius, apertureRadius, 0);

                //Vector3 lensOffset = Vector3(0, 0, 0);

                //Vector3 rayDirection = lensOffset - pixelPos;
                Vector3 rayDirection = ThinLensTransformPoint(pixelPos, camera.Lens.FocalLength) - lensOffset;
                rayDirection /= rayDirection.Z; // normalize to a unit z step

                Vector3 startCamera = lensOffset + (-camera.Near) * rayDirection;
                // convert the start and end points to from [-1;1]^3 to [0;1]^3
                Vector3 start = TransformPoint(camera.Perspective, startCamera);
                start = BigToSmallCube(start);

                Vector3 endCamera = lensOffset + (-camera.Far) * rayDirection;
                Vector3 end = TransformPoint(camera.Perspective, endCamera);
                end = BigToSmallCube(end);

                //colorSum += intersectHeightField(start, end);
                //}
                //}
            }
            // convert from [0;1]^3 to [-1;1]^3
            static Vector3 SmallToBigCube(Vector3 vector)
            {
                return 2 * vector - new Vector3(1, 1, 1);
            }

            // convert from [-1;1]^3 to [0;1]^3
            static Vector3 BigToSmallCube(Vector3 vector)
            {
                return 0.5f * (vector + new Vector3(1, 1, 1));
            }

            static Vector3 TransformPoint(Matrix4 matrix, Vector3 point)
            {
                Vector4 result = Vector4.Transform(new Vector4(point, 1), matrix);
                return result.Xyz / result.W;
            }

            static Vector3 ThinLensTransformPoint(Vector3 point, float focalLength)
            {
                return point / (1.0f - (Math.Abs(point.Z) / focalLength));
            }
        }
    }
}
