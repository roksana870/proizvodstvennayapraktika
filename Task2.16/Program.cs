using System.Globalization;
using System.Text;
namespace Task2_16;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        var path=Path.Combine(Path.GetTempPath(),$"task2-16-{Guid.NewGuid():N}.txt");
        try
        {
            using(var writer=new FileWriter(path)) writer.WriteLine("Первая запись.");
            FileDemo.WriteSecondLine(path);
            // Эксклюзивное открытие проверяет, что оба StreamWriter уже закрыты.
            using(var exclusive=new FileStream(path,FileMode.Open,FileAccess.ReadWrite,FileShare.None))
                Console.WriteLine($"Файл доступен для повторного открытия: {exclusive.Length} байт.");
            Console.Write(File.ReadAllText(path));
        }
        finally { File.Delete(path); }
    }
}
public sealed class FileWriter:IDisposable
{
    private readonly StreamWriter writer;
    private bool disposed;
    public FileWriter(string path,bool append=false)=>writer=new StreamWriter(path,append,Encoding.UTF8);
    public void WriteLine(string text)
    {
        ObjectDisposedException.ThrowIf(disposed,this);
        writer.WriteLine(text);
    }
    public void Dispose()
    {
        if(disposed) return;
        writer.Dispose(); disposed=true;
    }
}
internal static class FileDemo
{
    public static void WriteSecondLine(string path)
    {
        using var writer=new FileWriter(path,append:true);
        writer.WriteLine("Вторая запись.");
    }
}
