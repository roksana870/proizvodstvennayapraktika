using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task1._11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите размер матрицы: ");

            if (!int.TryParse(Console.ReadLine(), out int n) || n < 2)
            {
                Console.WriteLine("Размер должен быть не меньше 2");
                return;
            }

            int[,] a = new int[n, n];
            Random r = new Random();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    a[i, j] = r.Next(1, 10);
                    Console.Write(a[i, j] + " ");
                }

                Console.WriteLine();
            }

            int sum = 0;

            for (int i = 0; i < n; i++)
            {
                sum += a[i, i];
            }

            Console.WriteLine("Сумма диагонали: " + sum);
        }
    }
}
