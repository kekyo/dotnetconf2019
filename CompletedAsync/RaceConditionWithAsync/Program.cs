using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace RaceConditionWithAsync
{
    class Program
    {
        private static async Task ReadIncrementAndWriteAsync()
        {
            using (var fs = new FileStream("data.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 65536, true))
            {
                var tr = new StreamReader(fs, Encoding.UTF8);
                var value = int.TryParse(await tr.ReadToEndAsync(), out var v) ? (v + 1) : 0;
                fs.Position = 0;
                var tw = new StreamWriter(fs, Encoding.UTF8);
                await tw.WriteLineAsync(value.ToString());
                await tw.FlushAsync();
            }
        }

        //static Task Main(string[] args) =>
        //    Task.WhenAll(Enumerable.Range(0, 100).Select(_ => ReadIncrementAndWriteAsync()));

        private static readonly object monitorLocker = new object();

        private static async Task ReadIncrementAndWriteAsyncLock()
        {
            // lock (monitorLocker)
            Monitor.Enter(monitorLocker);
            try
            {
                using (var fs = new FileStream("data.txt",
                    FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 65536, true))
                {
                    var tr = new StreamReader(fs, Encoding.UTF8);
                    var value = int.TryParse(await tr.ReadToEndAsync(), out var v) ? (v + 1) : 0;
                    fs.Position = 0;
                    var tw = new StreamWriter(fs, Encoding.UTF8);
                    await tw.WriteLineAsync(value.ToString());
                    await tw.FlushAsync();
                }
            }
            finally
            {
                Monitor.Exit(monitorLocker);
            }
        }

        //static Task Main(string[] args) =>
        //    Task.WhenAll(Enumerable.Range(0, 100).Select(_ => ReadIncrementAndWriteAsyncLock()));

        private static readonly AsyncLock asyncLocker = new AsyncLock();

        private static async Task ReadIncrementAndWriteAsyncLockAsync()
        {
            using (await asyncLocker.LockAsync())
            {
                using (var fs = new FileStream("data.txt",
                    FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 65536, true))
                {
                    var tr = new StreamReader(fs, Encoding.UTF8);
                    var value = int.TryParse(await tr.ReadToEndAsync(), out var v) ? (v + 1) : 0;
                    fs.Position = 0;
                    var tw = new StreamWriter(fs, Encoding.UTF8);
                    await tw.WriteLineAsync(value.ToString());
                    await tw.FlushAsync();
                }
            }
        }

        static Task Main(string[] args) =>
            Task.WhenAll(Enumerable.Range(0, 100).Select(_ => ReadIncrementAndWriteAsyncLockAsync()));
    }
}
