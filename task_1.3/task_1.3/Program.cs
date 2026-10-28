using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task_1._3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Первое число: ");
            double a = double.Parse(Console.ReadLine());

            Console.Write("Второе число: ");
            double b = double.Parse(Console.ReadLine());

            Console.Write("Операция (+, -, *, /): ");
            string znak = Console.ReadLine();

            switch (znak)
            {
                case "+":
                    Console.WriteLine(a + b);
                    break;

                case "-":
                    Console.WriteLine(a - b);
                    break;

                case "*":
                    Console.WriteLine(a * b);
                    break;

                case "/":
                    if (b == 0)
                    {
                        Console.WriteLine("На ноль делить нельзя");
                    }
                    else
                    {
                        Console.WriteLine(a / b);
                    }
                    break;

                default:
                    Console.WriteLine("Неизвестная операция");
                    break;
            }
        }
    }
}
