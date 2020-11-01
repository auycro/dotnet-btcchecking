using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace price_change_x_percent
{
  public class Program
  {
    public static void Main(string[] args)
    {
      DotNetEnv.Env.Load();
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
      string port = DotNetEnv.Env.GetString("APP_PORT") ?? "5000";
      string url = String.Concat("http://0.0.0.0:", port);

      return Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder.UseStartup<Startup>().UseUrls(url);
        })
        .ConfigureServices(services =>
        {
          services.AddHostedService<price_change_x_percent.Workers.BitflyerWorker>();
          services.AddHostedService<price_change_x_percent.Services.LineMessageService>();
        });
    }
  }
}
