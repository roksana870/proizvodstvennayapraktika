using System.Globalization;
using System.Text;
using System.Diagnostics;
namespace Task2_10;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        var shapes=new List<IShape>{new Circle(2),new Rectangle(3,4),new Triangle(3,4,5),new Circle(1),new Rectangle(2,2)};
        Console.WriteLine("До сортировки:");
        foreach(var shape in shapes) Console.WriteLine($"{shape}: площадь={shape.Area():F2}, периметр={shape.Perimeter():F2}");
        shapes.Sort((a,b)=>a.Area().CompareTo(b.Area()));
        Console.WriteLine("После сортировки:");
        foreach(var shape in shapes) Console.WriteLine($"{shape}: площадь={shape.Area():F2}, периметр={shape.Perimeter():F2}");
        if(Math.Abs(new Triangle(3,4,5).Area()-6)>1e-9) throw new Exception("Неверная площадь");
        try { _=new Triangle(1,2,3); throw new Exception("Вырожденный треугольник принят"); }
        catch(ArgumentException) { Console.WriteLine("Вырожденный треугольник отклонён."); }
    }
}
public interface IShape { double Area(); double Perimeter(); }
internal static class Dimensions
{
    public static double Positive(double value)=>double.IsFinite(value)&&value>0?value:throw new ArgumentOutOfRangeException(nameof(value));
}
public sealed class Circle : IShape
{
    public double Radius {get;}
    public Circle(double radius)=>Radius=Dimensions.Positive(radius);
    public double Area()=>Math.PI*Radius*Radius;
    public double Perimeter()=>2*Math.PI*Radius;
    public override string ToString()=>$"Круг r={Radius}";
}
public sealed class Rectangle : IShape
{
    public double Width {get;} public double Height {get;}
    public Rectangle(double width,double height) { Width=Dimensions.Positive(width); Height=Dimensions.Positive(height); }
    public double Area()=>Width*Height;
    public double Perimeter()=>2*(Width+Height);
    public override string ToString()=>$"Прямоугольник {Width} x {Height}";
}
public sealed class Triangle : IShape
{
    public double A{get;} public double B{get;} public double C{get;}
    public Triangle(double a,double b,double c)
    {
        A=Dimensions.Positive(a); B=Dimensions.Positive(b); C=Dimensions.Positive(c);
        if(a+b<=c||a+c<=b||b+c<=a) throw new ArgumentException("Нарушено неравенство треугольника.");
    }
    public double Perimeter()=>A+B+C;
    public double Area() { var s=Perimeter()/2; return Math.Sqrt(s*(s-A)*(s-B)*(s-C)); }
    public override string ToString()=>$"Треугольник {A}, {B}, {C}";
}
