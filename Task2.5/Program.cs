using System.Globalization;
using System.Text;
using System.Diagnostics;
namespace Task2_5;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        int a=1,b=7; GenericMethods.Swap(ref a,ref b);
        Console.WriteLine($"int: {a}, {b}; Max={GenericMethods.Max(a,b)}");
        string c="apple",d="pear"; GenericMethods.Swap(ref c,ref d);
        Console.WriteLine($"string: {c}, {d}; Max={GenericMethods.Max(c,d)}");
        double e=1.5,f=2.5; GenericMethods.Swap(ref e,ref f);
        Console.WriteLine($"double: {e}, {f}; Max={GenericMethods.Max(e,f)}");
    }
}
public static class GenericMethods
{
    public static void Swap<T>(ref T a,ref T b) => (a,b)=(b,a);
    public static T Max<T>(T a,T b) where T:IComparable<T>
    {
        if(a is null) return b;
        if(b is null) return a;
        return a.CompareTo(b)>=0?a:b;
    }
}
