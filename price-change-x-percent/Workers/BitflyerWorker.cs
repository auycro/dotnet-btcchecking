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
    static readonly Uri endpointUri = new Uri("https://api.bitflyer.com");

    static readonly int MAX_BARS = 100;

    private Queue<CandleStick> oneMinuteCandleSticks = new Queue<CandleStick>();
    private Queue<CandleStick> fiveMinuteCandleSticks = new Queue<CandleStick>();
    private Queue<CandleStick> thirtyMinuteCandleSticks = new Queue<CandleStick>();

    private Ticker previousTick = null;

    private readonly ILogger<BitflyerWorker> _logger;

    public BitflyerWorker(ILogger<BitflyerWorker> logger)
    {
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        Ticker ticker = await GetTicker();

        EnqueueCandleStick(ticker, oneMinuteCandleSticks, 1);
        EnqueueCandleStick(ticker, fiveMinuteCandleSticks, 5);
        //EnqueueCandleStick(ticker, thirtyMinuteCandleSticks, 30);
        
        _logger.LogInformation($"1m: {oneMinuteCandleSticks.Count}, 5m: {fiveMinuteCandleSticks.Count}");

        previousTick = ticker;

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

    public void EnqueueCandleStick(Ticker ticker, Queue<CandleStick> candleStickQueue, int minute_interval){
      CandleStick candleStick = candleStickQueue.LastOrDefault();

      //InitialQueue
      if (candleStick == default) {        
        candleStick = CandleStick.CreateCandleStick(ticker);
        candleStickQueue.Enqueue(candleStick);
        return;
      }

      if (candleStick.IsSameMinuteFrame(ticker.timestamp, minute_interval)) {
        candleStick.UpdateCandleStick(ticker);
      } else {
        candleStick = CandleStick.CreateCandleStick(ticker);
        candleStickQueue.Enqueue(candleStick);
      }
      
      //_logger.LogInformation($"{minute_interval}m: {candleStickQueue.Count}");
      //_logger.LogInformation($"{candleStickQueue.Count}:{candleStick.ToString()}");

      //Keep Queue Stable
      if (candleStickQueue.Count > MAX_BARS) {
        candleStickQueue.Dequeue();
      }
    }

    public void CheckCandleTrends(){
      
    }
  }
}
