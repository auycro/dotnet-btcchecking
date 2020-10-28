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
    public class BitflyerWorker : BackgroundService
    {
        private readonly ILogger<BitflyerWorker> _logger;
        static readonly Uri endpointUri = new Uri("https://api.bitflyer.com");

        private Stack<CandleStick> oneMinuteCandleSticks = new Stack<CandleStick>();
        private Stack<CandleStick> fiveMinuteCandleSticks = new Stack<CandleStick>();
        private Stack<CandleStick> thirtyMinuteCandleSticks = new Stack<CandleStick>();

        public BitflyerWorker(ILogger<BitflyerWorker> logger)
        {
          _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
          while (!stoppingToken.IsCancellationRequested)
          {
            Ticker ticker = await GetTicker();
            
            StackUpCandleStick(ticker, oneMinuteCandleSticks);

            await Task.Delay(1000, stoppingToken);
          }
        }

        private async Task<Ticker> GetTicker(){
          var method = "GET";
          var path = "/v1/getticker?product_code=FX_BTC_JPY";
          var query = "";

          using (var client = new HttpClient())
          using (var request = new HttpRequestMessage(new HttpMethod(method), path + query))
          {
            client.BaseAddress = endpointUri;
            var message = await client.SendAsync(request);
            var response = await message.Content.ReadAsStringAsync();

            //Json to Ticker
            Ticker ticker = Newtonsoft.Json.JsonConvert.DeserializeObject<Ticker>(response);
            return ticker;
          }
        }

        public void StackUpCandleStick(Ticker ticker, Stack<CandleStick> candleStickStack){
          CandleStick candleStick = CandleStick.CreateCandleStick(ticker);
          if (candleStickStack.Count > 0) {
            CandleStick old_candleStick = candleStickStack.Peek();
            if (old_candleStick.IsSameMinuteFrame(ticker.timestamp)){
              candleStickStack.Pop();
              old_candleStick.UpdateCandleStick(ticker);
              candleStick = old_candleStick;
            }
          }
          candleStickStack.Push(candleStick);
          //_logger.LogInformation($"{candleStickStack.Count}:{candleStick.ToString()}");
        }
    }
}
