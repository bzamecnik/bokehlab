﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BokehLab.DepthPeeling
{
    class Program
    {
        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            try
            {
                DepthPeeling.RunExample();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
