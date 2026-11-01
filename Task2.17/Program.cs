using System.Globalization;
using System.Text;
namespace Task2_17;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        var temperature=new Temperature { Celsius=25 };
        Console.WriteLine($"{temperature.Celsius:F2} C = {temperature.Fahrenheit:F2} F");
        temperature.Celsius=-273.15;
        Console.WriteLine($"Абсолютный ноль: {temperature.Celsius:F2} C = {temperature.Fahrenheit:F2} F");
        foreach(var invalid in new[]{-300d,double.NaN,double.PositiveInfinity})
        {
            try { temperature.Celsius=invalid; throw new Exception("Принято некорректное значение"); }
            catch(ArgumentOutOfRangeException) { Console.WriteLine($"Значение {invalid} отклонено."); }
        }
        Console.WriteLine($"Температура после ошибок: {temperature.Celsius:F2} C");
    }
}
public sealed class Temperature
{
    private double celsius;
    public double Celsius
    {
        get=>celsius;
        set
        {
            if(!double.IsFinite(value)||value< -273.15) throw new ArgumentOutOfRangeException(nameof(value),"Температура должна быть конечной и не ниже -273.15.");
            celsius=value;
        }
    }
    public double Fahrenheit=>Celsius*9/5+32;
}
