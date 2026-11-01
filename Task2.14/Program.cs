using System.Globalization;
using System.Text;
namespace Task2_14;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        foreach(var type in Enum.GetValues<NotificationType>())
            NotificationFactory.Create(type).Send("Задание выполнено.");
    }
}
public enum NotificationType { Email,Sms,Push }
public interface INotification { void Send(string message); }
// Учебные реализации выводят сообщение в консоль и не обращаются к внешним сервисам.
public sealed class EmailNotification:INotification { public void Send(string message)=>Console.WriteLine($"Email: {message}"); }
public sealed class SmsNotification:INotification { public void Send(string message)=>Console.WriteLine($"SMS: {message}"); }
public sealed class PushNotification:INotification { public void Send(string message)=>Console.WriteLine($"Push: {message}"); }
public static class NotificationFactory
{
    public static INotification Create(NotificationType type)=>type switch
    {
        NotificationType.Email=>new EmailNotification(),NotificationType.Sms=>new SmsNotification(),
        NotificationType.Push=>new PushNotification(),_=>throw new ArgumentOutOfRangeException(nameof(type))
    };
}
