using System.Globalization;
using System.Text;
namespace Task2_15;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        var account=new BankAccount();
        account.Deposit(1000); account.Withdraw(300);
        Console.WriteLine($"После успешного снятия: {account.Balance:F2}");
        try { account.Withdraw(1000); }
        catch(InsufficientFundsException ex) { Console.WriteLine(ex.Message); }
        Console.WriteLine($"После отклонённой операции: {account.Balance:F2}");
        if(account.Balance!=700) throw new Exception("Баланс изменился при ошибке.");
        try { account.Withdraw(-1); throw new Exception("Принята отрицательная сумма"); }
        catch(ArgumentOutOfRangeException) { Console.WriteLine("Отрицательная сумма отклонена."); }
    }
}
public sealed class InsufficientFundsException:Exception
{
    public InsufficientFundsException(decimal requested,decimal available):
        base($"Недостаточно средств: запрошено {requested:F2}, доступно {available:F2}.") { }
}
public sealed class BankAccount
{
    private decimal balance;
    public decimal Balance=>balance;
    public void Deposit(decimal amount)
    {
        if(amount<=0) throw new ArgumentOutOfRangeException(nameof(amount));
        balance=checked(balance+amount);
    }
    public void Withdraw(decimal amount)
    {
        if(amount<=0) throw new ArgumentOutOfRangeException(nameof(amount));
        if(amount>balance) throw new InsufficientFundsException(amount,balance);
        balance-=amount;
    }
}
