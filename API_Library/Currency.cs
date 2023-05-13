using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace API_Library
{
    public class AlphaVantageResponse
    {
        [JsonProperty("Realtime Currency Exchange Rate")]
        public ExchangeRate ExchangeRate { get; set; }
    }

    public class ExchangeRate
    {
        [JsonProperty("1. From_Currency Code")]
        public string FromCurrencyCode { get; set; }

        [JsonProperty("2. From_Currency Name")]
        public string FromCurrencyName { get; set; }

        [JsonProperty("3. To_Currency Code")]
        public string ToCurrencyCode { get; set; }

        [JsonProperty("4. To_Currency Name")]
        public string ToCurrencyName { get; set; }

        [JsonProperty("5. Exchange Rate")]
        public string Rate { get; set; }

        [JsonIgnore]
        public string RateString => Rate.TrimEnd('0');

        [JsonProperty("6. Last Refreshed")]
        public DateTime LastRefreshed { get; set; }

        [JsonProperty("7. Time Zone")]
        public string TimeZone { get; set; }

        [JsonProperty("8. Bid Price")]
        public string BidPrice { get; set; }

        [JsonIgnore]
        public string BidPriceString => BidPrice.TrimEnd('0');

        [JsonProperty("9. Ask Price")]
        public string AskPrice { get; set; }

        [JsonIgnore]
        public string AskPriceString => AskPrice.TrimEnd('0');

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            LastRefreshed = TimeZoneInfo.ConvertTimeFromUtc(LastRefreshed, TimeZoneInfo.Local);
        }
    }
}
