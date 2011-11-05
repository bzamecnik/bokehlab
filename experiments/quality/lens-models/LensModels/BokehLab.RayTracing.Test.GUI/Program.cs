using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BokehLab.RayTracing.Test.GUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new ComplexLensLrtfForm());
            try
            {
                Application.Run(new HeighFieldForm());
                //Application.Run(new ComplexLensForm());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
