using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IndependentLibrary
{
    public interface IDownloader
    {
        Task<JToken> DownloadJsonAsync(string url);
    }

    public sealed class Downloader : IDownloader
    {
        public async Task<JToken> DownloadJsonAsync(string url)
        {
            var httpClient = new HttpClient();
            using (var hs = await httpClient.GetStreamAsync(url))
            {
                var jr = new JsonTextReader(new StreamReader(hs, Encoding.UTF8));
                var serializer = new JsonSerializer();
                return serializer.Deserialize<JToken>(jr);
            }
        }
    }
}
