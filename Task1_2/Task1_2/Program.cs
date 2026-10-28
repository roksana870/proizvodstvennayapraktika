using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int age = 25;
            long bigNumber = 10000000000;
            double pi = 3.14;
            float height = 1.75f;
            decimal price = 99.99m;
            bool isStudent = true;
            char grade = 'A';
            string name = "Алексей навальный";

            Console.WriteLine($"age = {age} ({age.GetType().Name})");
            Console.WriteLine($"bigNumber = {bigNumber} ({bigNumber.GetType().Name})");
            Console.WriteLine($"pi = {pi} ({pi.GetType().Name})");
            Console.WriteLine($"height = {height} ({height.GetType().Name})");
            Console.WriteLine($"price = {price} ({price.GetType().Name})");
            Console.WriteLine($"isStudent = {isStudent} ({isStudent.GetType().Name})");
            Console.WriteLine($"grade = {grade} ({grade.GetType().Name})");
            Console.WriteLine($"name = {name} ({name.GetType().Name})");
        }
    }
}
