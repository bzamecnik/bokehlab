using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BokehLab.LrtfVisualizer
{
    class Program
    {
        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            LrtfVisualizerGpu.RunExample();
        }
    }
}
