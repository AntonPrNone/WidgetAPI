using MongoDB.Driver;
using System.Threading.Tasks;

namespace WidgetAPI
{
    internal class MongoDbClient
    {
        private static readonly string _connectionString = "mongodb://localhost:27017";
        private static readonly IMongoClient _client = new MongoClient(_connectionString);
        private static readonly IMongoDatabase _database = _client.GetDatabase("DB");
        private static readonly IMongoCollection<User> _collection = _database.GetCollection<User>("User");

        public static async Task<User> GetUserAsync(string login)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Login, login);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public static async Task AddUserAsync(User user)
        {
            await _collection.InsertOneAsync(user);
        }

        public static async Task ReplaceUserAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Login, user.Login);
            await _collection.ReplaceOneAsync(filter, user);
        }
    }
}
