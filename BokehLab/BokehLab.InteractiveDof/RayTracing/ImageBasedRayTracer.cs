namespace BokehLab.InteractiveDof.RayTracing
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using BokehLab.InteractiveDof;
    using BokehLab.InteractiveDof.DepthPeeling;
    using BokehLab.InteractiveDof.MultiViewAccum;
    using BokehLab.Math;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    class ImageBasedRayTracer : IncrementalRenderer
    {
        static readonly string VertexShaderPath = "RayTracing/IbrtVS.glsl";
        static readonly string FragmentShaderPath = "RayTracing/IbrtFS.glsl";

        int vertexShader;
        int fragmentShader;
        int shaderProgram;

        int[] lensSamplesTextures;
        int pixelSamplesTexture;

        // number of samples rendered in a single rendering cycle
        int lensSampleCount = 2 * 2;
        int lensSampleTileSize = 128;
        int totalSampleCount;
        float totalSampleCountInv;

        // total number of rendering cycles to be incrementally accumulated
        static readonly int SingleFrameIterations = 32;

        Matrix4 sensorTransform;
        float[] sensorTransform3x3;

        public DepthPeeler DepthPeeler { get; set; }

        public ImageBasedRayTracer()
            : base(SingleFrameIterations)
        {
            ViewsPerFrame = 1;
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
            GL.BindTexture(TextureTarget.Texture3D, lensSamplesTextures[iteration]);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture1D, pixelSamplesTexture);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, DepthPeeler.PackedDepthTextures[0]);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, DepthPeeler.ColorTextures[0]);
            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, DepthPeeler.ColorTextures[1]);
            GL.ActiveTexture(TextureUnit.Texture5);
            GL.BindTexture(TextureTarget.Texture2D, DepthPeeler.ColorTextures[2]);
            GL.ActiveTexture(TextureUnit.Texture6);
            GL.BindTexture(TextureTarget.Texture2D, DepthPeeler.ColorTextures[3]);

            // enable IBRT shader
            GL.UseProgram(shaderProgram);

            // set shader parameters (textures, lens model, ...)
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "lensSamplesTexture"), 0);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "pixelSamplesTexture"), 1);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "packedDepthTexture0"), 2);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "colorTexture0"), 3);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "colorTexture1"), 4);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "colorTexture2"), 5);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "colorTexture3"), 6);

            GL.Uniform2(GL.GetUniformLocation(shaderProgram, "sensorSize"), camera.SensorSize);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "sensorZ"), camera.SensorZ);
            //GL.Uniform3(GL.GetUniformLocation(shaderProgram, "sensorShift"), camera.SensorShift3);
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
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "sampleCount"), totalSampleCount);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "sampleCountInv"), totalSampleCountInv);

            GL.Uniform2(GL.GetUniformLocation(shaderProgram, "cameraShift"), camera.LensShift);
            GL.UniformMatrix3(GL.GetUniformLocation(shaderProgram, "sensorTransform"), 1, false, sensorTransform3x3);

            // draw the quad
            LayerHelper.DrawQuad();

            // disable shader
            GL.UseProgram(0);

            // unbind textures
            GL.ActiveTexture(TextureUnit.Texture6);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture5);
            GL.BindTexture(TextureTarget.Texture2D, 0);
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
            if (camera.SensorRotation.X > 0)
            {
                sensorTransform *= Matrix4.CreateRotationX(camera.SensorRotation.X);
            }
            if (camera.SensorRotation.Y > 0)
            {
                sensorTransform *= Matrix4.CreateRotationY(camera.SensorRotation.Y);
            }
            sensorTransform3x3 = Matrix4x4To3x3Array(sensorTransform, ref sensorTransform3x3);
        }

        public override void Initialize(int width, int height)
        {
            base.Initialize(width, height);

            ShaderLoader.CreateShaderFromFiles(
               VertexShaderPath, FragmentShaderPath,
               out vertexShader, out fragmentShader, out shaderProgram);

            GL.Enable(EnableCap.Texture2D);

            RegenerateSampleTextures(lensSampleTileSize, lensSampleCount);
        }

        public override void Dispose()
        {
            if (shaderProgram != 0)
                GL.DeleteProgram(shaderProgram);
            if (vertexShader != 0)
                GL.DeleteShader(vertexShader);
            if (fragmentShader != 0)
                GL.DeleteShader(fragmentShader);

            if (lensSamplesTextures != null)
                GL.DeleteTextures(SingleFrameIterations, lensSamplesTextures);

            base.Dispose();
        }

        private void RegenerateSampleTextures(int tileSize, int sampleCount)
        {
            int sqrtSampleCount = (int)Math.Sqrt(sampleCount);
            totalSampleCount = sqrtSampleCount * sqrtSampleCount;
            totalSampleCountInv = 1 / (float)totalSampleCount;

            if ((lensSamplesTextures == null) || (lensSamplesTextures[0] == 0))
            {
                lensSamplesTextures = new int[SingleFrameIterations];
                GL.GenTextures(SingleFrameIterations, lensSamplesTextures);
            }
            if (pixelSamplesTexture == 0)
            {
                pixelSamplesTexture = GL.GenTexture();
            }

            for (int i = 0; i < SingleFrameIterations; i++)
            {
                GenerateLensSamplesTexture(lensSamplesTextures[i], tileSize, sqrtSampleCount, totalSampleCount);
            }
            GeneratePixelSamplesTexture(pixelSamplesTexture, sqrtSampleCount, totalSampleCount);
        }

        private void GenerateLensSamplesTexture(int textureId, int tileSize, int sqrtSampleCount, int totalSampleCount)
        {
            GL.BindTexture(TextureTarget.Texture3D, textureId);
            // size of a group of samples for a single pixel
            int bands = 2;
            int groupSize = bands * totalSampleCount;
            int textureSize = groupSize * tileSize * tileSize;

            Sampler sampler = new Sampler();
            IntPtr texturePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * textureSize);
            unsafe
            {
                int zStride = bands * tileSize * tileSize;
                for (int y = 0; y < tileSize; y++)
                {
                    for (int x = 0; x < tileSize; x++)
                    {
                        float* row = (float*)texturePtr + bands * (y * tileSize + x);
                        int index = 0;
                        // Z dimension, totalSampleCount times
                        foreach (Vector2d sample in
                            sampler.GenerateJitteredSamples(sqrtSampleCount))
                        {
                            Vector2d lensPos = Sampler.ConcentricSampleDisk(sample);
                            //2 * (sample - new Vector2d(0.5, 0.5));
                            row[index] = (float)lensPos.X;
                            row[index + 1] = (float)lensPos.Y;
                            index += zStride;
                        }
                    }
                }
            }

            // TODO: could be an half float or unsigned byte instead of a float
            // TODO: two sample pair could be stored in one 4-channel value
            GL.TexImage3D(TextureTarget.Texture3D, 0, PixelInternalFormat.Rg32f,
                tileSize, tileSize, totalSampleCount, 0,
                PixelFormat.Rg, PixelType.Float, texturePtr);

            // TODO: when to unallocate the buffer?

            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Clamp);
        }

        private void GeneratePixelSamplesTexture(int textureId, int sqrtSampleCount, int totalSampleCount)
        {
            GL.BindTexture(TextureTarget.Texture1D, textureId);
            // size of a group of samples for a single pixel
            int bands = 2;
            int textureSize = bands * totalSampleCount;

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
                totalSampleCount, 0,
                PixelFormat.Rg, PixelType.HalfFloat, texturePtr);

            // TODO: when to unallocate the buffer?

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
