using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task1._4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите число: ");
            int a = int.Parse(Console.ReadLine());

            Console.WriteLine("Способ 1: " + Sposob1(a));
            Console.WriteLine("Способ 2: " + Sposob2(a));
        }

        static string Sposob1(int a)
        {
            if (a % 2 == 0)
            {
                return "Четное";
            }
            else
            {
                return "Нечетное";
            }
        }

        static string Sposob2(int a)
        {
            return a % 2 == 0 ? "Четное" : "Нечетное";
        }
    }
    
}
