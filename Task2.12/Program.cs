using System.Globalization;
using System.Text;
namespace Task2_12;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        var students=new List<Student>{new("Ольга",4.5m),new("Анна",4.9m),new("Иван",3.8m),new("Борис",4.5m)};
        students.Sort();
        Console.WriteLine("По баллу:");
        foreach(var student in students) Console.WriteLine(student);
        students.Sort(new ByNameComparer());
        Console.WriteLine("По имени:");
        foreach(var student in students) Console.WriteLine(student);
    }
}
public sealed class Student:IComparable<Student>
{
    public string Name {get;} public decimal AverageGrade {get;}
    public Student(string name,decimal averageGrade)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if(averageGrade<0||averageGrade>5) throw new ArgumentOutOfRangeException(nameof(averageGrade));
        Name=name; AverageGrade=averageGrade;
    }
    public int CompareTo(Student? other)
    {
        if(other is null) return 1;
        var comparison=other.AverageGrade.CompareTo(AverageGrade);
        return comparison!=0?comparison:StringComparer.CurrentCulture.Compare(Name,other.Name);
    }
    public override string ToString()=>$"{Name}: {AverageGrade:F1}";
}
public sealed class ByNameComparer:IComparer<Student>
{
    public int Compare(Student? x,Student? y)
    {
        if(ReferenceEquals(x,y)) return 0;
        if(x is null) return -1;
        if(y is null) return 1;
        return StringComparer.CurrentCulture.Compare(x.Name,y.Name);
    }
}
