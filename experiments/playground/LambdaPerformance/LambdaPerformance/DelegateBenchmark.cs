using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LambdaPerformance
{
    interface IFoo
    {
        int Foo(int x);
    }
    class DelegateBenchmark : IFoo
    {
        const int Iterations = 1000000000;

        public int Foo(int x)
        {
            return x * 3;
        }

        public static void Run()
        {
            int x = 3;
            IFoo ifoo = new DelegateBenchmark();
            Func<int, int> del = ifoo.Foo;
            //Func<int, int> del = (value) => value * 3;
            // Make sure everything's JITted:
            ifoo.Foo(3);
            del(3);

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < Iterations; i++)
            {
                x = ifoo.Foo(x);
            }
            sw.Stop();
            Console.WriteLine("Interface: {0}", sw.ElapsedMilliseconds);

            x = 3;
            sw = Stopwatch.StartNew();
            for (int i = 0; i < Iterations; i++)
            {
                x = del(x);
            }
            sw.Stop();
            Console.WriteLine("Delegate: {0}", sw.ElapsedMilliseconds);
        }
    }
}
