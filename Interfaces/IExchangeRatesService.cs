using exRate.Models.DBModels;

namespace exRate.Interfaces
{
    public interface IExchangeRatesService
    {
        /// <summary>
        /// Функция, которая вернёт курсы валют от currency
        /// </summary>
        /// <param name="currency">Валюта, курс которой мы будем смотреть</param>
        /// <returns></returns>
        //Task<List<ExchangeRate>> GetCurrentRate(string currency);
        Task<List<ExchangeRate>> GetCurrentRates(string currency);

        
    }
}
