using System.Globalization;
using System.Text;
namespace Task2_13;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        var first=new Point(1,2); var second=new Point(1,2); var third=new Point(2,3);
        Console.WriteLine($"Одинаковые координаты равны: {first.Equals(second)}");
        Console.WriteLine($"Equals(object): {first.Equals((object)second)}");
        Console.WriteLine($"Одинаковые хеш-коды: {first.GetHashCode()==second.GetHashCode()}");
        var points=new HashSet<Point>{first,second,third};
        Console.WriteLine($"Уникальных точек: {points.Count}");
        if(points.Count!=2 || first.Equals(null) || first.Equals(third)) throw new Exception("Ошибка равенства");
    }
}
public sealed class Point(int x,int y):IEquatable<Point>
{
    // Неизменяемые координаты: хеш не меняется после добавления в HashSet.
    public int X {get;}=x; public int Y {get;}=y;
    public bool Equals(Point? other)=>other is not null && X==other.X && Y==other.Y;
    public override bool Equals(object? obj)=>obj is Point other && Equals(other);
    public override int GetHashCode()=>HashCode.Combine(X,Y);
    public override string ToString()=>$"({X}, {Y})";
}
