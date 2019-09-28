using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;

namespace InterestingService
{
    public interface IService
    {
        void Execute(int parameter);
    }

    public sealed class LoggingService : IService
    {
        public void Execute(int parameter) =>
            Trace.WriteLine($"Parameter={parameter}");
    }

    public sealed class BackgroundLoggingService : IService
    {
        public void Execute(int parameter) =>
            ThreadPool.QueueUserWorkItem(_ =>
                Trace.WriteLine($"Parameter={parameter}"));
    }

    public sealed class MakeMoneyService : IService
    {
        public async void Execute(int parameter)
        {
            var httpClient = new HttpClient();
            using (var response = await httpClient.PostAsync(
                walletServiceUrl, new StringContent($"money:{parameter}")))
            {
                Trace.WriteLine($"Result={response.StatusCode}");
            }
        }

        private static readonly string walletServiceUrl =
            "https://example.com/api/make";
    }
}
