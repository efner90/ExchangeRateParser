using exRate.Interfaces;
using exRate.Models.DBModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;

namespace exRate
{
    public class CoinMarketCapExchangeRateService : IExchangeRatesService
    {
        /// <summary>
        /// Апи ключ для доступа к коинмаркету
        /// </summary>
        private string _apiKey;
        /// <summary>
        /// Адрес к API коинмаркета
        /// </summary>
        readonly string _url;
        /// <summary>
        /// указываем ресурс, с которого тянем валюты в классе CoinMarketCapExchangeRateService
        /// </summary>
        private static string _rateSource = "coin_market_cap";

        readonly string _urlEndPoint;


        private readonly ILogger<CoinMarketCapExchangeRateService> _logger;
        private readonly PgWriter _writer;

        public CoinMarketCapExchangeRateService(IConfiguration config, ILogger<CoinMarketCapExchangeRateService> logger, PgWriter pgWriter)
        {
            _logger = logger;
            _apiKey = config["EXCHANGE_RATE_TOKEN"];
            _url = config["EXCHANGE_RATE_URL"];
            _urlEndPoint = config["EXCHANGE_RATE_URL_ENDPOINT"];
            _writer = pgWriter;
            _logger.LogInformation("CoinMarketCapExchangeRateService inialize");
        }
        /// <summary>
        /// возвращает лист конвертируемых валют
        /// </summary>
        /// <param name="currency">Валюта, которую нужно конвертировать</param>
        /// <returns>Лист валют</returns>
        public async Task<List<ExchangeRate>> GetCurrentRates(string currency) 
        {
            return await GetDataFromService(currency);
        }
        /// <summary>
        /// Возвращает список валют с сервиса, которые конвертируем к currency
        /// </summary>
        /// <param name="currency">валюта, список конвертации которой получаем</param>
        /// <returns></returns>
        private async Task<List<ExchangeRate>> GetDataFromService(string currency)
        {
            var uri = new UriBuilder();
            uri.Scheme = "https";
            uri.Host = _url;
            uri.Path = _urlEndPoint;

            var URL = uri;            

            //var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/tools/price-conversion"); // ендпоинт, который должен быть в идеальном мире, но не работает, /todo на будущее

            var queryString = HttpUtility.ParseQueryString(string.Empty);           

            queryString["start"] = "1"; //c какого номера в списке начинаем перебор
            queryString["limit"] = "120"; //дефолтное значение в 120                
            queryString["convert"] = currency;


            URL.Query = queryString.ToString();

            using (var client = new WebClient())
            {
                client.Headers.Add("X-CMC_PRO_API_KEY", _apiKey);
                client.Headers.Add("Accepts", "application/json");
                var result = client.DownloadString(URL.ToString());
                var jsonData = JObject.Parse(result);

                List<ExchangeRate> exRateList = new();

                foreach (var data in jsonData["data"])
                {
                    var exchangeRate = new ExchangeRate();

                    try
                    {
                        exchangeRate.From = currency;
                        exchangeRate.To = data["symbol"].ToString();
                        double rate = double.Parse(data["quote"][exchangeRate.From]["price"].ToString());
                        exchangeRate.Rate = SplitString(rate);
                        exchangeRate.Rate_Source = _rateSource;
                        DateTime localTime = DateTime.Parse(data["last_updated"].ToString());                        
                        var preTime = DateTime.Parse(data["last_updated"].ToString());
                        exchangeRate.LastUpdateTimeValue = new DateTime(preTime.Year, preTime.Month, preTime.Day
                            , preTime.Hour, preTime.Minute, preTime.Second, DateTimeKind.Utc); //явно указываем время в utc
                        exchangeRate.TimeStamp = DateTime.UtcNow;

                        if(exchangeRate.LastUpdateTimeValue > exchangeRate.TimeStamp)
                        {
                            _logger.LogError("Error with time: Last update time can't be more over TimeStamp");
                        }
                        exRateList.Add(exchangeRate);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in rate proccesing");
                    }

                }
                
                _logger.LogInformation("Get db from list in count {RatesCount}", exRateList.Count);
                return exRateList;
            }
        } 
        /// <summary>
        /// Разделяет тип дабл на составляющие до и после запятой, а после режет количество после запятой до 7
        /// </summary>
        /// <param name="teststring">переменная типа дабл, которую надо отформатировать</param>
        /// <returns>число дабл с 7 знаками после запятой</returns>
        private double SplitString(double teststring)
        {
            var stringText = teststring.ToString();
            //разделяем по точке
            string[] exchangeRateParts = stringText.Split(',');

            // Проверим, что у нас есть дробная часть и она содержит более 7 символов
            if (exchangeRateParts.Length > 1 && exchangeRateParts[1].Length > 7)
            {
                // Ограничим количество символов после запятой до 7
                exchangeRateParts[1] = exchangeRateParts[1].Substring(0, 7);
            }

            // Соединим обратно в строку с помощью точки
            string exchangeRateFormattedStr = string.Join(",", exchangeRateParts);

            // Преобразуем в double
            double exchangeRate = double.Parse(exchangeRateFormattedStr);
            return exchangeRate;
        }
    }
}
