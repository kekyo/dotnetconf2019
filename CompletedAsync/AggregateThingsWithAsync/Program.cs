using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AggregateThingsWithAsync
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // 気象庁防災情報XMLフォーマット形式電文
            // http://xml.kishou.go.jp/xmlpull.html
            var feedUrl =
                new Uri("http://www.data.jma.go.jp/developer/xml/feed/regular_l.xml", UriKind.RelativeOrAbsolute);

            var sw = new Stopwatch();
            sw.Start();

            //await DumpSequentialAsync(feedUrl);
            await DumpParallelAsync(feedUrl);

            sw.Stop();

            Console.WriteLine("================================");
            Console.WriteLine($"Elapsed={sw.Elapsed}");
        }

        private static readonly HttpClient httpClient = new HttpClient();

        private static async Task<XDocument> FetchXmlAsync(Uri url)
        {
            using (var fs = await httpClient.GetStreamAsync(url))
            {
                return await XDocument.LoadAsync(
                    fs, LoadOptions.None, CancellationToken.None);
            }
        }

        private static readonly XNamespace feedXmlns = "http://www.w3.org/2005/Atom";
        private static readonly XNamespace bodyXmlns = "http://xml.kishou.go.jp/jmaxml1/body/meteorology1/";

        private static async Task DumpSequentialAsync(Uri feedUrl)
        {
            var feedDocument = await FetchXmlAsync(feedUrl);

            foreach (var link in feedDocument.Root.
                Elements(feedXmlns + "entry").Elements(feedXmlns + "link"))
            {
                var href = (string)link.Attribute("href");
                if (!string.IsNullOrWhiteSpace(href))
                {
                    var jmaDocument = await FetchXmlAsync(new Uri(href, UriKind.RelativeOrAbsolute));

                    foreach (var text in jmaDocument.Root.
                        Elements(bodyXmlns + "Body").Elements(bodyXmlns + "Comment").
                        Elements(bodyXmlns + "Text"))
                    {
                        var commentText = (string)text;
                        Console.WriteLine(commentText);
                    }
                }
            }
        }

        private static async Task DumpParallelAsync(Uri feedUrl)
        {
            var feedDocument = await FetchXmlAsync(feedUrl);

            XElement[][] textLists = await Task.WhenAll(feedDocument.Root.
                Elements(feedXmlns + "entry").Elements(feedXmlns + "link").
                Select(async link =>
                {
                    var href = (string)link.Attribute("href");
                    var jmaDocument = await FetchXmlAsync(new Uri(href, UriKind.RelativeOrAbsolute));
                    XElement[] texts = jmaDocument.Root.
                        Elements(bodyXmlns + "Body").Elements(bodyXmlns + "Comment").
                        Elements(bodyXmlns + "Text").
                        ToArray();
                    return texts;
                }));

            foreach (var text in textLists.SelectMany(texts => texts))
            {
                var commentText = (string)text;
                Console.WriteLine(commentText);
            }
        }
    }
}
