using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using price_change_x_percent.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Line.Messaging;
using System.Threading.Tasks;

namespace price_change_x_percent.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class LineMessageController : ControllerBase
  {
    //private readonly ILogger<LineMessageController> _logger;
/*****
$ curl -v -X POST https://api.line.me/v2/bot/message/push \
-H 'Content-Type:application/json' \
-H 'Authorization: Bearer YOUR_ACCESS_TOKEN_HERE' \
-d '{"to": "YOUR_USER_ID_HERE","messages":[{"type": "text","text": "Hello, world"}]}'
******/
    private static LineMessagingClient lineMessagingClient;
    private string accessToken = Environment.GetEnvironmentVariable("ChannelAccessToken") ?? "xxxxxxxxxx";
    //private string channelSecret = Environment.GetEnvironmentVariable("ChannelSecret") ?? "xxxxxxxxxx";

    public LineMessageController(){
      lineMessagingClient = new LineMessagingClient(accessToken);
    }

    [HttpGet]
    public ActionResult Get()
    {
        return Content($"{StatusCodes.Status200OK}");
    }

    [HttpPost]
    public ActionResult Post([FromBody]JObject request)
    {
      Console.WriteLine(request.ToString());
      return Content($"{StatusCodes.Status200OK}");
    }

    [HttpPost("SayHello")]
    public async Task<ActionResult> SayHello([FromBody]JObject request){
      await lineMessagingClient.PushMessageAsync("xxxxxxxxxx",$"{request.ToString()}");
      return Content($"{StatusCodes.Status200OK}");
    }
  }
}