using Autofac;
using exRate.Interfaces;

namespace exRate
{
    public class ExchangeRateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CoinMarketCapExchangeRateService>().As<IExchangeRatesService>().SingleInstance();
            builder.RegisterType<PgWriter>().InstancePerDependency();
            
        }
    }
}
