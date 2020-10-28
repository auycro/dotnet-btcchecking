using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace price_change_x_percent.Models
{
  public class CandleStick
  {
    public long open_time { get; set;}
    public long close_time { get; set;}
    public long open_price { get; set;}
    public long high_price { get; set;}
    public long low_price { get; set;}
    public long close_price { get; set;}
    public long volume { get; set;}
    public long quote_volume {get; set;}

    public static CandleStick CreateCandleStick(Ticker ticker){
      CandleStick candleStick = new CandleStick();
      candleStick.open_time = ConvertUTCStringToEpoch(ticker.timestamp);
      candleStick.close_time = ConvertUTCStringToEpoch(ticker.timestamp);
      candleStick.open_price = ticker.ltp;
      candleStick.high_price = ticker.ltp;
      candleStick.low_price = ticker.ltp;
      candleStick.close_price = ticker.ltp;
      //candleStick.volume = ticker.volume;
      return candleStick;
    }

    public void UpdateCandleStick(Ticker ticker){
      close_time = ConvertUTCStringToEpoch(ticker.timestamp);
      high_price = (ticker.ltp > high_price)? ticker.ltp : high_price;
      low_price = (ticker.ltp < low_price)? ticker.ltp : low_price;
      close_price = ticker.ltp;
      //volume = volume + ticker.volume;          
    }

    public bool IsSameMinuteFrame(string timestamp, int minute_interval){
      DateTimeOffset previousOpenTick = StripEpochToMinuteAsDateTimeOffset(open_time);
      
      var epochMilliseconds = ConvertUTCStringToEpoch(timestamp);
      DateTimeOffset currentTick = StripEpochToMinuteAsDateTimeOffset(epochMilliseconds);

      var currentTimespan = currentTick.Subtract(previousOpenTick);
      TimeSpan timespan_interval = TimeSpan.FromMinutes(minute_interval);

      if (currentTimespan < timespan_interval) {
        return true;
      }

      return false;
    }

    private DateTimeOffset StripEpochToMinuteAsDateTimeOffset(long epochTimestamp){
      DateTimeOffset minuteFrame = DateTimeOffset.FromUnixTimeMilliseconds(epochTimestamp);
      minuteFrame = minuteFrame.AddMilliseconds(-minuteFrame.Millisecond);
      minuteFrame = minuteFrame.AddSeconds(-minuteFrame.Second);
      return minuteFrame;
    }

    private static long ConvertUTCStringToEpoch(string timestamp){
      DateTimeOffset utc_time = DateTimeOffset.Parse(timestamp).UtcDateTime;
      return utc_time.ToUnixTimeMilliseconds();
    }

    public override string ToString(){
      return  $"open_time:{open_time},"+
        $"close_time:{close_time},"+
        $"open_price:{open_price}," +
        $"high_price:{high_price}." +
        $"low_price:{low_price}," +
        $"close_price:{close_price}";
    } 
  }
}