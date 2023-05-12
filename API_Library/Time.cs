using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Library
{
    internal class Time
    {
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
    }
}
