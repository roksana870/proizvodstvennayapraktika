using System.Globalization;
using System.Text;
using System.Diagnostics;
namespace Task2_2;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        StringBenchmarks.BuildWithPlus(100); StringBenchmarks.BuildWithBuilder(100);
        var timer = Stopwatch.StartNew();
        var first = StringBenchmarks.BuildWithPlus(10000);
        timer.Stop();
        var plusTime = timer.Elapsed.TotalMilliseconds;
        timer.Restart();
        var second = StringBenchmarks.BuildWithBuilder(10000);
        timer.Stop();
        var builderTime = timer.Elapsed.TotalMilliseconds;
        Console.WriteLine($"Конкатенация: {plusTime:F3} мс");
        Console.WriteLine($"StringBuilder: {builderTime:F3} мс");
        Console.WriteLine($"Разница: {plusTime - builderTime:F3} мс");
        Console.WriteLine($"Результаты равны: {first == second}, длина: {first.Length}");
        if (first != second) throw new Exception("Разные строки.");
    }
}
internal static class StringBenchmarks
{
    public static string BuildWithPlus(int count)
    {
        var result = "";
        for (var i=0;i<count;i++) result += i.ToString(CultureInfo.InvariantCulture) + " ";
        return result;
    }
    public static string BuildWithBuilder(int count)
    {
        var result = new StringBuilder();
        for (var i=0;i<count;i++) result.Append(i.ToString(CultureInfo.InvariantCulture)).Append(' ');
        return result.ToString();
    }
    // string неизменяем: при + создаётся новая строка и копируется накопленный текст.
    // StringBuilder переиспользует буфер. Один замер иллюстративен и зависит от среды.
}
