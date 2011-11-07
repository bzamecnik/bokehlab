namespace BokehLab.InteractiveDof.RayTracing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using BokehLab.InteractiveDof;
    using BokehLab.InteractiveDof.DepthPeeling;
    using BokehLab.Math;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    class ImageBasedRayTracer : AbstractRendererModule
    {
        static readonly string VertexShaderPath = "RayTracing/IbrtVS.glsl";
        static readonly string FragmentShaderPath = "RayTracing/IbrtFS.glsl";

        int vertexShader;
        int fragmentShader;
        int shaderProgram;

        public DepthPeeler DepthPeeler { get; set; }

        public void DrawIbrtImage(Camera camera)
        {
            Debug.Assert(DepthPeeler != null);

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // bind color and depth textures
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, DepthPeeler.ColorTextures[0]);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, DepthPeeler.DepthTextures[0]);

            // enable IBRT shader
            GL.UseProgram(shaderProgram);

            // set shader parameters (textures, lens model, ...)
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "colorTexture"), 0); // TextureUnit.Texture0
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "depthTexture"), 1); // TextureUnit.Texture1
            GL.Uniform2(GL.GetUniformLocation(shaderProgram, "sensorSize"), camera.SensorSize);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "sensorZ"), camera.SensorZ);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "near"), camera.Near);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "far"), camera.Far);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "lensFocalLength"), camera.Lens.FocalLength);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "lensApertureRadius"), camera.Lens.ApertureRadius);
            //Matrix4 perspective = camera.Perspective;
            //GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "perspective"), false, ref perspective);
            GL.Uniform4(GL.GetUniformLocation(shaderProgram, "frustumBounds"), camera.FrustumBounds);

            // draw the quad
            LayerHelper.DrawQuad();

            // disable shader
            GL.UseProgram(0);

            // unbind textures
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public override void Initialize(int width, int height)
        {
            base.Initialize(width, height);

            ShaderLoader.CreateShaderFromFiles(
               VertexShaderPath, FragmentShaderPath,
               out vertexShader, out fragmentShader, out shaderProgram);

            GL.Enable(EnableCap.Texture2D);
        }

        public override void Dispose()
        {
            if (shaderProgram != 0)
                GL.DeleteProgram(shaderProgram);
            if (vertexShader != 0)
                GL.DeleteShader(vertexShader);
            if (fragmentShader != 0)
                GL.DeleteShader(fragmentShader);

            base.Dispose();
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
