using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace price_change_x_percent.Models
{
    public class CandleStick
    {
        public long close_time { get; set;}
        public long open_price { get; set;}
        public long high_price { get; set;}
        public long low_price { get; set;}
        public long close_price { get; set;}
        public long volume { get; set;}
        public long quote_volume {get; set;}

        public static CandleStick CreateCandleStick(Ticker ticker){
          CandleStick candleStick = new CandleStick();
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
          //open_price = ticker.ltp;
          high_price = (ticker.ltp > high_price)? ticker.ltp : high_price;
          low_price = (ticker.ltp < low_price)? ticker.ltp : low_price;
          close_price = ticker.ltp;
          //volume = volume + ticker.volume;          
        }

        public bool IsSameMinuteFrame(string timestamp){
          var epochMilliseconds = ConvertUTCStringToEpoch(timestamp);

          var this_time = StripEpochToMinute(close_time);
          var new_time = StripEpochToMinute(epochMilliseconds);

          if (this_time == new_time) {
            return true;
          }
          return false;
        }

        private long StripEpochToMinute(long epochTimestamp){
            DateTimeOffset minuteFrame = DateTimeOffset.FromUnixTimeMilliseconds(epochTimestamp);
            minuteFrame = minuteFrame.AddSeconds(-minuteFrame.Second);
            return minuteFrame.ToUnixTimeSeconds();
        }

        private static long ConvertUTCStringToEpoch(string timestamp){
          DateTimeOffset utc_time = DateTimeOffset.Parse(timestamp).UtcDateTime;
          return utc_time.ToUnixTimeMilliseconds();
        }

        public override string ToString(){
          return  $"close_time:{close_time},"+
            $"open_price:{open_price}," +
            $"high_price:{high_price}." +
            $"low_price:{low_price}," +
            $"close_price:{close_price}";
        } 
    }
}