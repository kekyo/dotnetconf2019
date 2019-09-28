using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileAccessWithAsync
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var fs = new FileStream(
                "sample.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.None,
                65536, true))    // Asynchronous I/O
            {
                var tw = new StreamWriter(fs, Encoding.UTF8);

                var total = 0;
                for (var index = 0; index < 100; index++)
                {
                    total += index;
                    await tw.WriteLineAsync($"Index={index}, Total={total}");
                }

                await tw.FlushAsync();
            }
        }

        static async Task AsyncSimulatedWriteSample1(string[] args)
        {
            using (var fs = File.Create("sample.txt"))
            {
                var tw = new StreamWriter(fs, Encoding.UTF8);   // or File.CreateText()

                var total = 0;
                for (var index = 0; index < 100; index++)
                {
                    total += index;
                    await tw.WriteLineAsync($"Index={index}, Total={total}");   // (Simulated)
                }

                await tw.FlushAsync();   // (Simulated)
            }
        }

        static async Task AsyncSimulatedWriteSample2(string[] args)
        {
            using (var fs = File.Create("sample.txt"))
            {
                var tw = new StreamWriter(fs, Encoding.UTF8);   // or File.CreateText()

                var total = 0;
                for (var index = 0; index < 100; index++)
                {
                    total += index;
                    await Task.Run(() => tw.WriteLine($"Index={index}, Total={total}"));   // (Simulated)
                }

                await Task.Run(() => tw.Flush());   // (Simulated)
            }
        }
    }
}


