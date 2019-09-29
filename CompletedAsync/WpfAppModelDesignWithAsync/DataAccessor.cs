using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppModelDesignWithAsync
{
    public static class DataAccessor
    {
        public static async Task<(int value0, int value1)> IncrementAsync(string path)
        {
            using (var fs = new FileStream(path,
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 65536, true))
            {
                var tr = new StreamReader(fs, Encoding.UTF8);
                var value0 = int.TryParse(
                    await tr.ReadToEndAsync().ConfigureAwait(false), out var v) ? v : 0;

                fs.Position = 0;
                var tw = new StreamWriter(fs, Encoding.UTF8);
                var value1 = value0 + 1;
                await tw.WriteLineAsync(value1.ToString());
                await tw.FlushAsync();

                return (value0, value1);
            }
        }
    }
}
