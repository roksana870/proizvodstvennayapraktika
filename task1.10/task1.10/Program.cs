using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task1._10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] a = new int[10000];
            Random r = new Random();

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = r.Next(1, 10001);
            }

            int[] b = (int[])a.Clone();
            int[] c = (int[])a.Clone();

            Stopwatch time = new Stopwatch();

            time.Start();
            BubbleSort(b);
            time.Stop();

            Console.WriteLine("Пузырьком: " + time.ElapsedMilliseconds + " мс");

            time.Restart();
            InsertionSort(c);
            time.Stop();

            Console.WriteLine("Вставками: " + time.ElapsedMilliseconds + " мс");

            bool ok = true;

            for (int i = 0; i < b.Length; i++)
            {
                if (b[i] != c[i])
                {
                    ok = false;
                    break;
                }

                if (i > 0 && (b[i] < b[i - 1] || c[i] < c[i - 1]))
                {
                    ok = false;
                    break;
                }
            }

            Console.WriteLine("Одинаково отсортированы: " + ok);
        }

        static void BubbleSort(int[] a)
        {
            for (int i = 0; i < a.Length - 1; i++)
            {
                for (int j = 0; j < a.Length - i - 1; j++)
                {
                    if (a[j] > a[j + 1])
                    {
                        int temp = a[j];
                        a[j] = a[j + 1];
                        a[j + 1] = temp;
                    }
                }
            }
        }

        static void InsertionSort(int[] a)
        {
            for (int i = 1; i < a.Length; i++)
            {
                int num = a[i];
                int j = i - 1;

                while (j >= 0 && a[j] > num)
                {
                    a[j + 1] = a[j];
                    j--;
                }

                a[j + 1] = num;
            }
        }
    }
}
