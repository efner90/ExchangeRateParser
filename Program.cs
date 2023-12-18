using Autofac;
using Autofac.Extensions.DependencyInjection;
using exRate;
using exRate.Extensions;
using exRate.Extensions.SharedUtils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog((ctx, lc) => SerilogCreator.Create(configuration, lc));

builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new ExchangeRateModule()));
builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterType<RunnerHostService>()
    .As<IHostedService>().SingleInstance()
    .WithParameter("args", args));

builder.Services.AddQuartz(q =>
{
    q.SchedulerId = "Scheduler-Core";
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.SetProperty("quartz.threadPool.threadCount", "1");
}
);
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

AutoFac.SetContaionerAutoFac(app.Services.GetAutofacRoot());

app.UseRouting();

await app.RunAsync();

await app.StopAsync(TimeSpan.FromSeconds(10));