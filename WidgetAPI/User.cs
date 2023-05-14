using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace WidgetAPI
{
    internal class User
    {
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Units { get; set; }
        public string Country { get; set; }
        public string CountryAbr { get; set; }
        public string City { get; set; }
        public string From_CurrencyCode  { get; set; }
        public string To_CurrencyCode  { get; set; }

        public Dictionary<string, bool> WidgetsOfInterest { get; set; } = new Dictionary<string, bool>
        {
            { "Cats", true },
            { "Weather", true },
            { "Currency", true },
            { "NewsWidget", true }
        };
    }
}
