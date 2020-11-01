using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Line.Messaging;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace price_change_x_percent.Services
{
  public class LineMessageService: IHostedService
  {
    private static LineMessagingClient lineMessagingClient;
    private string accessToken = DotNetEnv.Env.GetString("LINE_CHANNEL_ACCESS_TOKEN") ?? "xxxxxxxxxx";
    private static string mainUserID = DotNetEnv.Env.GetString("LINE_USER_ID") ?? "xxxxxxxxxx"; 

    private readonly ILogger<LineMessageService> _logger;

    public LineMessageService(ILogger<LineMessageService> logger)
    {
      _logger = logger;
      lineMessagingClient = new LineMessagingClient(accessToken);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      //throw new NotImplementedException();
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      //throw new NotImplementedException();
      return Task.CompletedTask;
    }

    //private string channelSecret = Environment.GetEnvironmentVariable("ChannelSecret") ?? "xxxxxxxxxx";

    /*****
    $ curl -v -X POST https://api.line.me/v2/bot/message/push \
    -H 'Content-Type:application/json' \
    -H 'Authorization: Bearer YOUR_ACCESS_TOKEN_HERE' \
    -d '{"to": "YOUR_USER_ID_HERE","messages":[{"type": "text","text": "Hello, world"}]}'
    ******/
    public static async Task SendLineMessage(string[] text){
      try {
        await lineMessagingClient.PushMessageAsync(mainUserID, text);
      } catch(Exception e){
        Console.WriteLine(e.Message);
      }
    }
  }
}