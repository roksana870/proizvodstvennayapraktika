using System.Globalization;
using System.Text;
namespace Task2_19;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        string?[] words={"арбуз","банан","апельсин","вишня","груша","ананас","брусника",
            "виноград","дыня","голубика","абрикос","ежевика","",null};
        var groups=words.Where(w=>!string.IsNullOrWhiteSpace(w))
            .Select(w=>w!.Trim().ToLowerInvariant())
            .GroupBy(w=>w[0]).OrderBy(g=>g.Key);
        foreach(var group in groups)
            Console.WriteLine($"{group.Key}: {string.Join(", ",group.OrderBy(w=>w,StringComparer.CurrentCulture))}");
    }
}
