using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using price_change_x_percent.Models;

namespace price_change_x_percent.Workers
{
    public class LineMessageWorker : BackgroundService
    {
        private readonly ILogger<LineMessageWorker> _logger;
        public LineMessageWorker(ILogger<LineMessageWorker> logger)
        {
          _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
          while (!stoppingToken.IsCancellationRequested)
          {
            await Task.Delay(1000, stoppingToken);
          }
        }
    }
}