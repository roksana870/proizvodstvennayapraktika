using System.Globalization;
using System.Text;
using System.Diagnostics;
namespace Task2_3;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        foreach (var text in new[] { "123", "abc", "999999999999999999999" })
        {
            Console.WriteLine($"TryParse({text}): {NumberParser.TryRead(text, out var a)}, {a}");
            Console.WriteLine($"Parse({text}): {NumberParser.TryReadWithCatch(text, out var b)}, {b}");
        }
    }
}
public static class NumberParser
{
    public static bool TryRead(string? text,out int value) => int.TryParse(text,out value);
    public static bool TryReadWithCatch(string? text,out int value)
    {
        value=0;
        try { value=int.Parse(text!); return true; }
        catch (FormatException) { return false; }
        catch (OverflowException) { return false; }
        catch (ArgumentNullException) { return false; }
    }
}
