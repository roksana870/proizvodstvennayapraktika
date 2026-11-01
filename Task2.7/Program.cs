using System.Globalization;
using System.Text;
using System.Diagnostics;
namespace Task2_7;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        Console.WriteLine($"Sum(1, 2, 3) = {Calculator.Sum(1,2,3)}");
        Console.WriteLine($"Sum(new[] {{1,2,3}}) = {Calculator.Sum(new[]{1,2,3})}");
        Console.WriteLine($"Sum() = {Calculator.Sum()}");
        Console.WriteLine($"Перегрузка IEnumerable: {Calculator.Sum((IEnumerable<int>)new[]{1,2,3})}");
    }
}
public static class Calculator
{
    // params не является частью сигнатуры. Sum(int[]) уже существует в форме params.
    // Поэтому настоящая перегрузка принимает IEnumerable<int>.
    public static int Sum(params int[] numbers)=>Sum((IEnumerable<int>)numbers);
    public static int Sum(IEnumerable<int> numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        var total=0;
        foreach(var number in numbers) total=checked(total+number);
        return total;
    }
}
