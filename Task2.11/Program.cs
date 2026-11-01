using System.Globalization;
using System.Text;
namespace Task2_11;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        foreach(var strategy in new IDiscountStrategy[]{new NoDiscount(),new VipDiscount(),new PromoDiscount()})
        {
            var calculator=new PriceCalculator(strategy);
            Console.WriteLine($"{strategy.GetType().Name}: {calculator.Calculate(1000):F2}");
        }
    }
}
public interface IDiscountStrategy { decimal Apply(decimal price); }
public sealed class NoDiscount:IDiscountStrategy { public decimal Apply(decimal price)=>price; }
public sealed class VipDiscount:IDiscountStrategy { public decimal Apply(decimal price)=>price*0.90m; }
public sealed class PromoDiscount:IDiscountStrategy { public decimal Apply(decimal price)=>price*0.80m; }
public sealed class PriceCalculator
{
    private readonly IDiscountStrategy strategy;
    public PriceCalculator(IDiscountStrategy strategy)=>this.strategy=strategy??throw new ArgumentNullException(nameof(strategy));
    public decimal Calculate(decimal price)
    {
        if(price<0) throw new ArgumentOutOfRangeException(nameof(price));
        return decimal.Round(strategy.Apply(price),2,MidpointRounding.AwayFromZero);
    }
}
