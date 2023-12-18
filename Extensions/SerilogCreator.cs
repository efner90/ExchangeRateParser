using System.Reflection;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace exRate.Extensions
{
    public static class SerilogCreator
    {
        public static void Create(IConfiguration config, LoggerConfiguration lc)
        {
            var seqAddress = "http://127.0.0.1:5341";
            var programName = "exchangeRateMiner";
            lc
                .MinimumLevel.Debug()
                .Enrich.WithProperty("ProgramName", programName)
                .Enrich.WithProperty("ProgramVersion", Assembly.GetExecutingAssembly().GetName().Version.ToString())
                .Enrich.FromLogContext()
                .WriteTo.Console(LogEventLevel.Information)
                .WriteTo.Seq(seqAddress, LogEventLevel.Debug);
        }
    }
}
