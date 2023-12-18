using System.ComponentModel.DataAnnotations.Schema;

namespace exRate.Models.DBModels
{
    [Table("exchange_rates")]
    public class ExchangeRate
    {
        //ключевое поле
        [Column("id")]
        public int Id { get; set; }

        //изначальная валюта        
        [Column("from", TypeName = "text")] 
        public string From { get; set; }

        //валюта, в которую конвертируем
        [Column("to", TypeName = "text")]  
        public string To { get; set; }

        //стоимость валюты from к to
        [Column("rate", TypeName = "double precision")]
        public double Rate { get; set; }
        // ресурс, с которого берём курсы валют
        [Column("rate_source", TypeName = "text")]
        public string Rate_Source { get; set; }
        //последнее обновление валюты
        [Column("requested_at", TypeName = "timestamp with time zone")] //штамп with time zone для валидного проливания в базу данных, необходимость в utc
        public DateTime LastUpdateTimeValue { get; set; }

        //время, когда получили
        [Column("timestamp", TypeName = "timestamp with time zone")]
        public DateTime TimeStamp { get; set; }
    } 
}
