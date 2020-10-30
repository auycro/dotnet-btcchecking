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
    static readonly float ALERT_PERCENT = 0.02f;

    private Queue<CandleStick> oneMinuteCandleSticks = new Queue<CandleStick>();
    private Queue<CandleStick> fiveMinuteCandleSticks = new Queue<CandleStick>();
    private Queue<CandleStick> thirtyMinuteCandleSticks = new Queue<CandleStick>();

    enum MARKET_STAGE {STEADY, UP, DOWN, FLUCTUATE};

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
      if (minute_interval == 1) {
      //  _logger.LogInformation($"{candleStickQueue.Count}:{candleStick.ToString()}");
        await AlertByPercent(candleStick, ticker.ltp);
      }

      //Keep Queue Stable
      if (candleStickQueue.Count > MAX_BARS) {
        candleStickQueue.Dequeue();
      }
    }

    public async Task AlertByPercent(CandleStick candleStick, long current_price){
      string sendText = string.Empty;
      double diff = 0.00f;

      //Up Trend Alert
      if (candleStick.open_price < current_price){
        diff = CalculateChangePercent(candleStick.open_price, current_price);
        if (Math.Abs(diff) > ALERT_PERCENT)
          sendText = $"Price Alert: Up {diff.ToString("0.00")} {candleStick.open_price.ToString("0.00")} {current_price.ToString("0.00")}";
      }

      //Down Trend Alert
      if (candleStick.open_price > current_price){
        diff = CalculateChangePercent(candleStick.open_price, current_price);
        if (Math.Abs(diff) > ALERT_PERCENT)
          sendText = $"Price Alert: Down {diff.ToString("0.00")} {candleStick.open_price.ToString("0.00")} {current_price.ToString("0.00")}";
      }

      if (!string.IsNullOrEmpty(sendText)) {
        _logger.LogInformation($"SendToLine: {sendText}");
        await LineMessageService.SendLineMessage(sendText);
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
