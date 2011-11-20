namespace BokehLab.InteractiveDof
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public class ShaderLoader
    {
        /// <summary>
        /// Create a shader program with a single vertex and and single
        /// fragment shader given paths to shader source codes.
        /// </summary>
        /// <param name="vsPath"></param>
        /// <param name="fsPath"></param>
        /// <param name="vertexShaderObject"></param>
        /// <param name="fragmentShaderObject"></param>
        /// <param name="shaderProgram"></param>
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
                vertexShaderObject = CreateShader(vs.ReadToEnd(), ShaderType.VertexShader);
                fragmentShaderObject = CreateShader(fs.ReadToEnd(), ShaderType.FragmentShader);
                shaderProgram = CreateShaderProgram(new[] { vertexShaderObject }, new[] { fragmentShaderObject });
            }
        }

        /// <summary>
        /// Craete a shader program 
        /// </summary>
        /// <param name="vsPaths"></param>
        /// <param name="fsPaths"></param>
        /// <param name="vertexShaderObjects"></param>
        /// <param name="fragmentShaderObjects"></param>
        /// <param name="shaderProgram"></param>
        public static void CreateShadersFromFiles(
            IEnumerable<string> vsPaths,
            IEnumerable<string> fsPaths,
            out IList<int> vertexShaderObjects,
            out IList<int> fragmentShaderObjects,
            out int shaderProgram)
        {
            Debug.Assert((vsPaths != null) || (vsPaths.Count() > 0));
            Debug.Assert((fsPaths != null) || (fsPaths.Count() > 0));

            vertexShaderObjects = new List<int>(vsPaths.Count());
            foreach (var path in vsPaths)
            {
                string source = File.ReadAllText(path);
                int vsObject = CreateShader(source, ShaderType.VertexShader);
                vertexShaderObjects.Add(vsObject);
            }

            fragmentShaderObjects = new List<int>(fsPaths.Count());
            foreach (var path in fsPaths)
            {
                string source = File.ReadAllText(path);
                int fsObject = CreateShader(source, ShaderType.FragmentShader);
                fragmentShaderObjects.Add(fsObject);
            }

            shaderProgram = CreateShaderProgram(vertexShaderObjects, fragmentShaderObjects);
        }

        public static int CreateShaderProgram(IEnumerable<int> vertexObjects, IEnumerable<int> fragmentObjects)
        {
            int shaderProgram = GL.CreateProgram();

            foreach (int fragmentObject in fragmentObjects)
            {
                GL.AttachShader(shaderProgram, fragmentObject);
            }
            foreach (int vertexObject in vertexObjects)
            {
                GL.AttachShader(shaderProgram, vertexObject);
            }

            GL.LinkProgram(shaderProgram);

            return shaderProgram;
        }

        /// <summary>
        /// Create a single shader from its source code.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int CreateShader(string source, ShaderType type)
        {
            int statusCode;
            string info;

            int shaderObject = GL.CreateShader(type);

            GL.ShaderSource(shaderObject, source);
            GL.CompileShader(shaderObject);
            GL.GetShaderInfoLog(shaderObject, out info);
            GL.GetShader(shaderObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
            {
                throw new ApplicationException(info);
            }

            return shaderObject;
        }
    }
}
