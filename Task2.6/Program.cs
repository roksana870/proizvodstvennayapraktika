using System.Globalization;
using System.Text;
using System.Diagnostics;
namespace Task2_6;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        foreach(var day in Enum.GetValues<DayOfWeek>())
            Console.WriteLine($"{Days.GetRussianName(day)}: выходной={Days.IsWeekend(day)}");
    }
}
public enum DayOfWeek { Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday }
public static class Days
{
    public static bool IsWeekend(DayOfWeek day)
    {
        if(!Enum.IsDefined(day)) throw new ArgumentOutOfRangeException(nameof(day));
        return day is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }
    public static string GetRussianName(DayOfWeek day)=>day switch
    {
        DayOfWeek.Monday=>"Понедельник",DayOfWeek.Tuesday=>"Вторник",
        DayOfWeek.Wednesday=>"Среда",DayOfWeek.Thursday=>"Четверг",
        DayOfWeek.Friday=>"Пятница",DayOfWeek.Saturday=>"Суббота",
        DayOfWeek.Sunday=>"Воскресенье",_=>throw new ArgumentOutOfRangeException(nameof(day))
    };
}
