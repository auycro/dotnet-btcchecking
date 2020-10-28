namespace price_change_x_percent.Models
{
    //https://www.nuget.org/packages/Line.Messaging/
    //https://developers.line.biz/en/docs/messaging-api/sending-messages/#methods-of-sending-message
    public class LineSendMessage
    {
      public string to { get; set; }
      public Message message { get; set; }
    }

    public class Message{
      public string message_type { get; set; }
      public string message_text { get; set; }
    }
}