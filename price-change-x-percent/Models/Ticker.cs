namespace price_change_x_percent.Models
{
    //https://lightning.bitflyer.com/docs?lang=en#ticker
    public class Ticker
    {
      public string product_code { get; set; }
      public string state { get; set; }
      public string timestamp { get; set; }
      public int tick_id { get; set; }
      public string best_bid { get; set; }
      public string best_ask { get; set; }
      public string best_bid_size { get; set; }
      public string best_ask_size { get; set; }
      public string total_bid_depth { get; set; }
      public string total_ask_depth { get; set; }
      public string market_bid_size { get; set; }
      public string market_ask_size { get; set; }
      public long ltp { get; set; }
      public long volume { get; set; } //fetch data is decimal but long is enough
      public string volume_by_product { get; set; }
    }
}