using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using price_change_x_percent.Workers;

namespace price_change_x_percent.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkerController : ControllerBase
    {
      private readonly ILogger<WorkerController> _logger;

      public WorkerController(ILogger<WorkerController> logger)
      {
        _logger = logger;
      }

    [HttpGet("alert_status")]
    public ActionResult GetStatus() => Content($"AlertStatus: {BitflyerWorker.AlertStatus}");

    [HttpGet("alert_on")]
    public ActionResult SetAlertOn(){
      BitflyerWorker.AlertStatus = true;
      _logger.LogInformation($"SET ALERT ON");
      return Content($"AlertStatus: {BitflyerWorker.AlertStatus}");
    }

    [HttpGet("alert_off")]
    public ActionResult SetAlertOff(){
      BitflyerWorker.AlertStatus = false;
      _logger.LogInformation($"SET ALERT OFF");
      return Content($"AlertStatus: {BitflyerWorker.AlertStatus}");
    }

  }
}