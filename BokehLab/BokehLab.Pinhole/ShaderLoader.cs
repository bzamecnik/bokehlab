namespace BokehLab.Pinhole
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public class ShaderLoader
    {
        public static void CreateShaderFromFiles(
            string vsPath,
            string fsPath,
            out int vertexShaderObject,
            out int fragmentShaderObject,
            out int shaderProgram)
        {
            using (StreamReader vs = new StreamReader(vsPath))
            using (StreamReader fs = new StreamReader(fsPath))
            {
                CreateShaders(vs.ReadToEnd(), fs.ReadToEnd(),
                    out vertexShaderObject, out fragmentShaderObject,
                    out shaderProgram);
            }
        }

        /// <summary>
        /// Create vertex and fragment shaders from source codes.
        /// </summary>
        /// <param name="vs">vertex shader source code</param>
        /// <param name="fs">fragment shader source code</param>
        /// <param name="vertexObject">vertex shader object id</param>
        /// <param name="fragmentObject">fragment shader object id</param>
        /// <param name="program">composite shader program id</param>
        public static void CreateShaders(
            string vs,
            string fs,
            out int vertexObject,
            out int fragmentObject,
            out int shaderProgram)
        {
            int statusCode;
            string info;

            vertexObject = GL.CreateShader(ShaderType.VertexShader);
            fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(vertexObject, vs);
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, out info);
            GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            // Compile vertex shader
            GL.ShaderSource(fragmentObject, fs);
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(fragmentObject, out info);
            GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, fragmentObject);
            GL.AttachShader(shaderProgram, vertexObject);

            GL.LinkProgram(shaderProgram);
        }
    }
}
