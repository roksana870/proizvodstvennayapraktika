using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task1._7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите текст: ");
            string text = Console.ReadLine() ?? "";

            string result = "";

            for (int i = text.Length - 1; i >= 0; i--)
            {
                result += text[i];
            }

            Console.WriteLine("Результат: " + result);
        }
    }
}
