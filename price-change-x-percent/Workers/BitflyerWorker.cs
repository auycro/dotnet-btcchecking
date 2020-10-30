using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using price_change_x_percent.Models;
using price_change_x_percent.Services;

namespace price_change_x_percent.Workers
{
  public class BitflyerWorker : BackgroundService
  {
    static readonly Uri endpointUri = new Uri("https://api.bitflyer.com");

    static readonly int MAX_BARS = 100;
    static readonly double[] ALERT_PERCENT = {0.5,1.0};

    private Queue<CandleStick> oneMinuteCandleSticks = new Queue<CandleStick>();
    private Queue<CandleStick> fiveMinuteCandleSticks = new Queue<CandleStick>();
    private Queue<CandleStick> thirtyMinuteCandleSticks = new Queue<CandleStick>();

    enum MARKET_TREND {DEFAULT, UP, DOWN, FLUCTUATE};

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

        await EnqueueCandleStick(ticker, oneMinuteCandleSticks, 1);
        await EnqueueCandleStick(ticker, fiveMinuteCandleSticks, 5);
        await EnqueueCandleStick(ticker, thirtyMinuteCandleSticks, 30);
        
        //_logger.LogInformation($"1m: {oneMinuteCandleSticks.Count}, "+
        //  $"5m: {fiveMinuteCandleSticks.Count}, "+
        //  $"30m: {thirtyMinuteCandleSticks.Count}");

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
        Ticker ticker = JsonConvert.DeserializeObject<Ticker>(response);
        return ticker;
      }
    }

    public async Task EnqueueCandleStick(Ticker ticker, Queue<CandleStick> candleStickQueue, int minute_interval){
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
      //if (minute_interval == 5) {
      //  _logger.LogInformation($"{candleStickQueue.Count}:{candleStick.ToString()}");
        //await AlertByPercent(candleStick, ticker.ltp, minute_interval);
      //}
      if (minute_interval >1)
        await AlertByPercent(candleStick, ticker.ltp, minute_interval);

      //Keep Queue Stable
      if (candleStickQueue.Count > MAX_BARS) {
        candleStickQueue.Dequeue();
      }
    }

    public async Task AlertByPercent(CandleStick candleStick, long current_price, int minute_interval){
      List<string> sendText = new List<string>();
      long compare_price = (candleStick.alert_price == 0)? candleStick.open_price : candleStick.alert_price;
      double diff = CalculateChangePercent(current_price, compare_price);
      
      double alert_percent = (minute_interval < 30)? ALERT_PERCENT[0] : ALERT_PERCENT[1];

      if (Math.Abs(diff) > alert_percent) {
        string diff_text = (diff > 0)? $"+{diff.ToString("0.00")}%":$"{diff.ToString("0.00")}%";
        sendText.Add($"PRICE ALERT [{minute_interval}m]: {diff_text}\n{compare_price}jpy -> {current_price}jpy");
        candleStick.alert_price = current_price;
      }

      if (sendText.Count > 0) {
        _logger.LogInformation($"SendToLine: {sendText.ToArray()[0]}");
        await LineMessageService.SendLineMessage(sendText.ToArray());
      }
    }

    public double CalculateChangePercent(long compare_price, long current_price){
      double diff = 0.00f;
      long base_price = (compare_price > current_price)? compare_price : current_price;
      diff = ((double)compare_price - current_price)/base_price * 100;
      //_logger.LogInformation($"CalculateChangePercent: {diff.ToString("0.000")}");
      return diff;
    }
  }
}
