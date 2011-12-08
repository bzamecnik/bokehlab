using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LambdaPerformance
{
    class Thresholding3d
    {
        public static void Run()
        {
            var data = PrepareData(512);
            float threshold = 0.5f;
            var inlineOutput = ComputeInline(data, threshold);
            var functionOutput = ComputeWithFunction(data, threshold);
            var lambdaOutput = ComputeWithLambda(data, threshold);

            //Console.WriteLine("compare (inline == function): {0}",
            //    Compare(inlineOutput, functionOutput));
            //Console.WriteLine("compare (inline == lambda): {0}",
            //    Compare(inlineOutput, lambdaOutput));
        }

        private static float[, ,] ComputeInline(float[, ,] data, float threshold)
        {
            int size = data.GetLength(0);
            Debug.Assert(size == data.GetLength(1));
            Debug.Assert(size == data.GetLength(2));

            float[, ,] output = new float[size, size, size];

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        output[i, j, k] = (data[i, j, k] > threshold) ? 1 : 0;
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("inline: {0} ms", sw.ElapsedMilliseconds);
            return output;
        }

        private static float[, ,] ComputeWithFunction(float[, ,] data, float threshold)
        {
            int size = data.GetLength(0);
            Debug.Assert(size == data.GetLength(1));
            Debug.Assert(size == data.GetLength(2));

            float[, ,] output = new float[size, size, size];

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        output[i, j, k] = Threshold(data[i, j, k], threshold);
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("function: {0} ms", sw.ElapsedMilliseconds);
            return output;
        }

        private static float Threshold(float value, float threshold)
        {
            return (value > threshold) ? 1 : 0;
        }

        private static float[, ,] ComputeWithLambda(float[, ,] data, float threshold)
        {
            int size = data.GetLength(0);
            Debug.Assert(size == data.GetLength(1));
            Debug.Assert(size == data.GetLength(2));

            float[, ,] output = new float[size, size, size];
            Func<float, float> thresholdFunc = (value) => (value > threshold) ? 1 : 0;

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        output[i, j, k] = thresholdFunc(data[i, j, k]);
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("lambda: {0} ms", sw.ElapsedMilliseconds);
            return output;
        }

        private static float[, ,] PrepareData(int size)
        {
            Random random = new Random();
            float[, ,] data = new float[size, size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        data[i, j, k] = (float)random.NextDouble();
                    }
                }
            }
            return data;
        }

        private static bool Compare(float[, ,] dataA, float[, ,] dataB)
        {
            int size = dataA.GetLength(0);
            Debug.Assert(size == dataA.GetLength(1));
            Debug.Assert(size == dataA.GetLength(2));
            Debug.Assert(size == dataB.GetLength(0));
            Debug.Assert(size == dataB.GetLength(1));
            Debug.Assert(size == dataB.GetLength(2));

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        if (dataA[i, j, k] != dataB[i, j, k])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

    }
}
