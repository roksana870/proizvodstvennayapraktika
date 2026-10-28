using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task1._14
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите дату рождения (дд.мм.гггг): ");
            string text = Console.ReadLine();

            DateTime date;

            if (!DateTime.TryParseExact(text, "dd.MM.yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out date))
            {
                Console.WriteLine("Неверная дата");
                return;
            }

            DateTime now = DateTime.Today;

            if (date > now)
            {
                Console.WriteLine("Дата рождения не может быть в будущем");
                return;
            }

            int years = now.Year - date.Year;

            if (date.AddYears(years) > now)
            {
                years--;
            }

            DateTime a = date.AddYears(years);

            int months = 0;

            while (a.AddMonths(1) <= now)
            {
                a = a.AddMonths(1);
                months++;
            }

            int days = (now - a).Days;

            Console.WriteLine(years + " лет, " + months +
                              " месяцев, " + days + " дней");
        }
    }
}
