using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LambdaPerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            //Thresholding2d.Run();
            DelegateBenchmark.Run();
        }
    }
}
