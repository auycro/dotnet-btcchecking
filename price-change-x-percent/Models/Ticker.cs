namespace price_change_x_percent.Models
{
     public class Ticker
    {
      public string product_code { get; set; }
      public string state { get; set; }
      public string timestamp { get; set; }
      public string tick_id { get; set; }
      public string best_bid { get; set; }
      public string best_ask { get; set; }
      public string best_bid_size { get; set; }
      public string best_ask_size { get; set; }
      public string total_bid_depth { get; set; }
      public string total_ask_depth { get; set; }
      public string market_bid_size { get; set; }
      public string market_ask_size { get; set; }
      public string ltp { get; set; }
      public string volume { get; set; }
      public string volume_by_product { get; set; }
    }
}