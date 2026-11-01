using System.Globalization;
using System.Text;
namespace Task2_18;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        var employees=new List<Employee>
        {
            new("Анна",32,65000,"Разработка"),new("Пётр",38,72000,"Разработка"),
            new("Мария",27,55000,"Разработка"),new("Иван",24,48000,"Разработка"),
            new("Ольга",35,61000,"Аналитика"),new("Дмитрий",29,58000,"Аналитика"),
            new("Елена",26,52000,"Аналитика"),new("Сергей",31,45000,"Тестирование"),
            new("Наталья",30,43000,"Тестирование"),new("Алексей",42,90000,"Менеджмент")
        };
        Console.WriteLine("Топ-3 по зарплате:");
        foreach(var e in employees.OrderByDescending(e=>e.Salary).ThenBy(e=>e.Name).Take(3))
            Console.WriteLine($"{e.Name}: {e.Salary:F2}");
        Console.WriteLine("Средняя зарплата по отделу:");
        foreach(var group in employees.GroupBy(e=>e.Department).OrderBy(g=>g.Key))
            Console.WriteLine($"{group.Key}: {group.Average(e=>e.Salary):F2}");
        Console.WriteLine("Старше 30:");
        foreach(var e in employees.Where(e=>e.Age>30).OrderBy(e=>e.Name))
            Console.WriteLine($"{e.Name}: {e.Age}");
    }
}
public sealed class Employee(string name,int age,decimal salary,string department)
{
    public string Name {get;}=name;
    public int Age {get;}=age;
    public decimal Salary {get;}=salary;
    public string Department {get;}=department;
}
