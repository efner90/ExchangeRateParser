namespace exRate.Models
{
    public class ExchangeModel
    {

        /// <summary>
        /// Валюта, которую мы берём за основу и буем смотреть. Конвертируемая
        /// </summary>
        public string CurrencyFrom { get; set; }
        /// <summary>
        /// Валюта, которую мы получим из валюты From
        /// </summary>
        public string CurrencyTo { get; set; }
        /// <summary>
        /// Количество валюты, которые мы полчаем из FROM в TO
        /// </summary>
        public double Rate { get; set; }
        /// <summary>
        /// Ресурс, с которого мы берём данные о валютах
        /// </summary>
        public string Rate_Source { get; set; }
        /// <summary>
        /// Время запроса к базе за валютами
        /// </summary>
        public DateTime Requested_at { get; set; }
        /// <summary>
        /// Время формирования таблицы
        /// </summary>
        public DateTime TimeStamp { get; set; }


    }
}
