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
    private readonly ILogger<LineMessageController> _logger;

    public LineMessageController(ILogger<LineMessageController> logger){
      _logger = logger;
    }

    [HttpGet]
    public ActionResult Get()
    {
        return Content($"{StatusCodes.Status200OK}");
    }

    [HttpPost("webhooks")]
    public ActionResult WebhookEndpoint([FromBody]JObject request)
    {
      _logger.LogInformation($"Received: {request.ToString()}");
      return Content($"{StatusCodes.Status200OK}");
    }
  }
}