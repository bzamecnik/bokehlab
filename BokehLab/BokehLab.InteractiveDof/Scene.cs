using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using BokehLab.MeshLoading;

namespace BokehLab.InteractiveDof
{
    class Scene
    {
        public VboObject vbo = VboObject.CreateCube();

        public void Draw()
        {
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            vbo.Draw();
        }
    }
}
