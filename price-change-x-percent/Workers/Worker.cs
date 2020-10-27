using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using price_change_x_percent.Models;

namespace price_change_x_percent.Workers
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        static readonly Uri endpointUri = new Uri("https://api.bitflyer.com");
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var method = "GET";
            var path = "/v1/getticker?product_code=FX_BTC_JPY";
            var query = "";

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(new HttpMethod(method), path + query))
                {
                    client.BaseAddress = endpointUri;
                    var message = await client.SendAsync(request);
                    var response = await message.Content.ReadAsStringAsync();

                    Ticker ticker = Newtonsoft.Json.JsonConvert.DeserializeObject<Ticker>(response);
                    _logger.LogInformation($"best_bid:{ticker.best_bid}");
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
