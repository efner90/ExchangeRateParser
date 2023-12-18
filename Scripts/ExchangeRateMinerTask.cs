using exRate.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;
using exRate.Extensions.SharedUtils;
using Microsoft.Extensions.Configuration;

namespace exRate.Scripts
{
    public class ExchangeRateMinerTask : IJob
    {
        private readonly ILogger <ExchangeRateMinerTask> _logger;
        private IExchangeRatesService _serviceCoinMarcetCap;
        private PgWriter _writer;
        private IConfiguration _config;

        public ExchangeRateMinerTask()
        {
            _logger = AutoFac.Resolve<ILogger<ExchangeRateMinerTask>>();
            _serviceCoinMarcetCap = AutoFac.Resolve<IExchangeRatesService>();
            _writer = AutoFac.Resolve<PgWriter>();
            _logger.LogInformation("ExchangeRateMinerTask initialized");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("ExchangeRateMinerTask started");

            var baseExchangeRate =  await _serviceCoinMarcetCap.GetCurrentRates("BTC");

            //добавляем в базу
            await _writer.AddRangeAsync(baseExchangeRate);            
            //сохраняем
            await _writer.SaveChangesAsync();

        }
    }
}
