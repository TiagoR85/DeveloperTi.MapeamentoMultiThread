using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class Program
{
    public static void Main()
    {
        var lista = Enumerable.Range(1, 100).ToList();
        var listList = lista.SplitList(10);
        var tasks = new List<Task<List<NumeroConvertido>>>();
        var returnList = new List<NumeroConvertido>();
        foreach (var list in listList)
        {
            tasks.Add(Task<List<NumeroConvertido>>.Factory.StartNew((_lista) =>
            {
                var internalLista = _lista as List<int>;
                return internalLista.Select(n => new NumeroConvertido
                {
                    Char = $"{n}",
                    CharId = n,
                    ThreadId = Thread.CurrentThread.ManagedThreadId
                }).ToList();
            }, list));
        }
        Task.WaitAll(tasks.ToArray());

        foreach (var task in tasks)
        {
            Console.WriteLine("Task.Id #{0}, ", task.Id);
            var ret = task.Result;
            foreach (var i in ret)
            {
                Console.WriteLine("Char {0}, CharId #{1}, ThreadId #{2}, ", i.Char, i.CharId, i.ThreadId);
            }
            returnList.AddRange(ret);
        }
        Console.ReadKey();
    }
}

public class NumeroConvertido
{
    public int CharId { get; set; }
    public string Char { get; set; }
    public int ThreadId { get; set; }
}

public static class ListExtensions
{
    public static List<List<T>> SplitList<T>(this List<T> items, int sliceSize = 30)
    {
        List<List<T>> list = new List<List<T>>();
        for (int i = 0; i < items.Count; i += sliceSize)
            list.Add(items.GetRange(i, Math.Min(sliceSize, items.Count - i)));
        return list;
    }
}