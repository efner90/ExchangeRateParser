using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using exRate.Scripts;

namespace exRate
{
    public class RunnerHostService : BackgroundService
    {
        private readonly ILogger<RunnerHostService> _logger;
        //private readonly IApiWorker _worker;
        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _sсheduler;

        public RunnerHostService(ILogger<RunnerHostService> logger, ISchedulerFactory schedulerFactory)
        {
            _logger = logger;
            _logger.LogInformation("RunnerHostService initialize");
            //_worker = apiWorker;
            _schedulerFactory = schedulerFactory;
            Task.Run(async () => await GetScheduler());
        }

        private async Task GetScheduler()
        {
            _sсheduler = await _schedulerFactory.GetScheduler();
        }

        /// <summary>
        /// Входная точка приложения
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //создаём джобу для работы с классом нашего сервиса
            IJobDetail exchangeRateJob = JobBuilder.Create<ExchangeRateMinerTask>()
                .WithIdentity("BTCMining", "mining")
                .Build();


            //создаём триггер, который будет пинать джобу каждый час в 00 и в 30 минут
            ITrigger twicePerHourTrigger = TriggerBuilder.Create()
                .WithIdentity("HourlyTrigger", "mining")
                 .StartNow()
                 .WithCronSchedule("0 0/30 * 1/1 * ? *") //каждый час каждые 30 минут
                 .Build();



            //создаём планировщик
            await _sсheduler.ScheduleJob(exchangeRateJob, twicePerHourTrigger);




        }
    }
}
