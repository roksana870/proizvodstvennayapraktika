using System.Globalization;
using System.Text;
using System.Diagnostics;
namespace Task2_8;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        foreach(var key in new[]{"one","missing"})
        {
            int? value=Storage.FindValue(key);
            Console.WriteLine($"{key}: значение по умолчанию={value ?? -1}");
            if(value.HasValue) Console.WriteLine($"Найдено: {value.Value}");
            else Console.WriteLine("Ключ не найден.");
        }
        string? name=null;
        Console.WriteLine($"Длина отсутствующей строки: {name?.Length ?? 0}");
        name="Анна";
        Console.WriteLine($"Длина имени: {name?.Length ?? 0}");
    }
}
public static class Storage
{
    private static readonly Dictionary<string,int> Values=new() {["one"]=1,["two"]=2};
    public static int? FindValue(string key)=>Values.TryGetValue(key,out var value)?value:null;
}
