using System.Globalization;
using System.Text;
using System.Diagnostics;
namespace Task2_1;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        foreach (var text in new[] { "А роза упала на лапу Азора", "Привет мир", "топот" })
            Console.WriteLine($"{text}: палиндром={text.IsPalindrome()}, слов={text.WordCount()}, наоборот={text.ReverseString()}");
        if ("".WordCount() != 0 || !"".IsPalindrome() || "A😀B".ReverseString() != "B😀A")
            throw new Exception("Ошибка обработки пустой строки или Unicode.");
    }
}
public static class StringExtensions
{
    // Пробелы и знаки препинания не учитываются, регистр не важен.
    public static bool IsPalindrome(this string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var symbols = text.EnumerateRunes().Where(Rune.IsLetterOrDigit).Select(Rune.ToUpperInvariant).ToArray();
        return symbols.SequenceEqual(symbols.Reverse());
    }
    public static int WordCount(this string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
    }
    public static string ReverseString(this string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var elements = new List<string>();
        var enumerator = StringInfo.GetTextElementEnumerator(text);
        while (enumerator.MoveNext()) elements.Add(enumerator.GetTextElement());
        elements.Reverse();
        return string.Concat(elements);
    }
}
