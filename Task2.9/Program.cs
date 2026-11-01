using System.Globalization;
using System.Text;
using System.Diagnostics;
namespace Task2_9;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        var animals=new List<Animal>{new Dog("Шарик"),new Cat("Мурка"),new Bird("Кеша")};
        foreach(var animal in animals) { Console.WriteLine(animal); animal.MakeSound(); }
        ((Bird)animals[2]).Fly();
    }
}
public abstract class Animal
{
    public string Name {get;}
    protected Animal(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name); Name=name;
    }
    public abstract void MakeSound();
    public override string ToString()=>$"{GetType().Name}: {Name}";
}
public sealed class Dog(string name):Animal(name)
{ public override void MakeSound()=>Console.WriteLine("Гав!"); }
public sealed class Cat(string name):Animal(name)
{ public override void MakeSound()=>Console.WriteLine("Мяу!"); }
public sealed class Bird(string name):Animal(name)
{
    public override void MakeSound()=>Console.WriteLine("Чирик!");
    public void Fly()=>Console.WriteLine($"{Name} летит.");
}
