using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task1._13
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите текст:");
            string text = (Console.ReadLine() ?? "").ToLower();

            char[] znaki = { ' ', ',', '.', '!', '?', ';', ':', '-', '\t' };
            string[] words = text.Split(znaki, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, int> a = new Dictionary<string, int>();

            foreach (string word in words)
            {
                if (a.ContainsKey(word))
                {
                    a[word]++;
                }
                else
                {
                    a[word] = 1;
                }
            }

            var list = new List<KeyValuePair<string, int>>(a);

            list.Sort((x, y) => y.Value.CompareTo(x.Value));

            foreach (var word in list)
            {
                Console.WriteLine(word.Key + " - " + word.Value);
            }
        }
    }
}
