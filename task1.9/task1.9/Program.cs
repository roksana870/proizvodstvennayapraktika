using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task1._9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] a = new int[20];
            Random r = new Random();

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = r.Next(1, 101);
            }

            Console.WriteLine("Массив:");
            Console.WriteLine(string.Join(" ", a));

            Console.WriteLine("Минимум: " + Min(a));
            Console.WriteLine("Максимум: " + Max(a));
            Console.WriteLine("Сумма: " + Sum(a));
            Console.WriteLine("Среднее: " + Avg(a));
        }

        static int Min(int[] a)
        {
            if (a.Length == 0) return 0;

            int min = a[0];

            for (int i = 1; i < a.Length; i++)
            {
                if (a[i] < min)
                    min = a[i];
            }

            return min;
        }

        static int Max(int[] a)
        {
            if (a.Length == 0) return 0;

            int max = a[0];

            for (int i = 1; i < a.Length; i++)
            {
                if (a[i] > max)
                    max = a[i];
            }

            return max;
        }

        static int Sum(int[] a)
        {
            int sum = 0;

            for (int i = 0; i < a.Length; i++)
            {
                sum += a[i];
            }

            return sum;
        }

        static double Avg(int[] a)
        {
            if (a.Length == 0) return 0;

            return (double)Sum(a) / a.Length;
        }
    }
}
