using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task1._6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] nums = { 1234, 567, -987 };

            foreach (int num in nums)
            {
                Console.WriteLine("Число: " + num);
                Console.WriteLine("Способ 1: " + Sposob1(num));
                Console.WriteLine("Способ 2: " + Sposob2(num));
                Console.WriteLine();
            }
        }

        static int Sposob1(int num)
        {
            string text = Math.Abs((long)num).ToString();
            int sum = 0;

            foreach (char c in text)
            {
                sum += int.Parse(c.ToString());
            }

            return sum;
        }

        static int Sposob2(int num)
        {
            long n = Math.Abs((long)num);
            int sum = 0;

            while (n > 0)
            {
                sum += (int)(n % 10);
                n /= 10;
            }

            return sum;
        }
    }
}
