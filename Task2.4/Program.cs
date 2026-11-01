using System.Globalization;
using System.Text;
using System.Diagnostics;
namespace Task2_4;
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        var stack = new MyStack<int>(1);
        var queue = new MyQueue<int>(1);
        for(var i=1;i<=6;i++) { stack.Push(i); queue.Enqueue(i); }
        Console.WriteLine($"Стек: Count={stack.Count}, Peek={stack.Peek()}");
        Console.WriteLine($"Очередь: Count={queue.Count}, Peek={queue.Peek()}");
        while(!stack.IsEmpty) Console.Write($"{stack.Pop()} ");
        Console.WriteLine();
        while(!queue.IsEmpty) Console.Write($"{queue.Dequeue()} ");
        Console.WriteLine();
        try { stack.Pop(); } catch(InvalidOperationException) { Console.WriteLine("Пустой стек: исключение обработано."); }
        try { queue.Dequeue(); } catch(InvalidOperationException) { Console.WriteLine("Пустая очередь: исключение обработано."); }
        var random=new Random(42);
        var expectedStack=new Stack<int>();
        var expectedQueue=new Queue<int>();
        for(var i=0;i<2000;i++)
        {
            if(expectedStack.Count==0 || random.Next(2)==0) { stack.Push(i); expectedStack.Push(i); }
            else if(stack.Pop()!=expectedStack.Pop()) throw new Exception("Ошибка LIFO");
            if(expectedQueue.Count==0 || random.Next(2)==0) { queue.Enqueue(i); expectedQueue.Enqueue(i); }
            else if(queue.Dequeue()!=expectedQueue.Dequeue()) throw new Exception("Ошибка FIFO");
            if(stack.Count!=expectedStack.Count || queue.Count!=expectedQueue.Count) throw new Exception("Ошибка Count");
            if(!stack.IsEmpty && stack.Peek()!=expectedStack.Peek()) throw new Exception("Ошибка Peek");
            if(!queue.IsEmpty && queue.Peek()!=expectedQueue.Peek()) throw new Exception("Ошибка Peek");
        }
        Console.WriteLine("2000 случайных шагов: результаты совпали со Stack и Queue.");
    }
}
public sealed class MyStack<T>
{
    private T[] items;
    public int Count {get;private set;}
    public bool IsEmpty=>Count==0;
    public MyStack(int capacity=4)
    {
        if(capacity<0) throw new ArgumentOutOfRangeException(nameof(capacity));
        items=new T[Math.Max(1,capacity)];
    }
    public void Push(T item)
    {
        if(Count==items.Length) Array.Resize(ref items,checked(items.Length*2));
        items[Count++]=item;
    }
    public T Peek()=>Count>0?items[Count-1]:throw new InvalidOperationException("Стек пуст.");
    public T Pop()
    {
        var value=Peek();
        items[--Count]=default!;
        return value;
    }
}
public sealed class MyQueue<T>
{
    private T[] items;
    private int head;
    public int Count {get;private set;}
    public bool IsEmpty=>Count==0;
    public MyQueue(int capacity=4)
    {
        if(capacity<0) throw new ArgumentOutOfRangeException(nameof(capacity));
        items=new T[Math.Max(1,capacity)];
    }
    public void Enqueue(T item)
    {
        if(Count==items.Length)
        {
            var expanded=new T[checked(items.Length*2)];
            for(var i=0;i<Count;i++) expanded[i]=items[(head+i)%items.Length];
            items=expanded; head=0;
        }
        items[(head+Count)%items.Length]=item; Count++;
    }
    public T Peek()=>Count>0?items[head]:throw new InvalidOperationException("Очередь пуста.");
    public T Dequeue()
    {
        var value=Peek(); items[head]=default!;
        head=(head+1)%items.Length; Count--; return value;
    }
}
