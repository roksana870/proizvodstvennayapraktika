using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task1._12
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите текст: ");
            string text = (Console.ReadLine() ?? "").ToLower();

            string a = "";

            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsLetterOrDigit(text[i]))
                {
                    a += text[i];
                }
            }

            string b = "";

            for (int i = a.Length - 1; i >= 0; i--)
            {
                b += a[i];
            }

            if (a == b)
            {
                Console.WriteLine("Палиндром: True");
            }
            else
            {
                Console.WriteLine("Палиндром: False");
            }
        }
    }
}
