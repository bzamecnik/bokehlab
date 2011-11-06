using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BokehLab.InteractiveDof
{
    interface IRendererModule
    {
        bool Enabled { get; set; }

        void Initialize(int width, int height);
        void Dispose();

        void Resize(int width, int height);
    }
}
