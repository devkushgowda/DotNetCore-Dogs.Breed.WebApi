using System;
using System.Collections.Generic;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dogs.Breed.WebApi.Database
{
    public class MongoDbConnectionStore
    {
        public const string ProfilesCollectionName = "profiles";

        public const string DogStoreDbName = "dogstore";

        private const string Password = "Kush@143";

        private const string Endpoint = "cluster0.0xitl.mongodb.net/dogstore?retryWrites=true&w=majority";

        static readonly string CloudConnectionString = $"mongodb+srv://admin:{HttpUtility.UrlEncode(Password)}@{Endpoint}";

        //static readonly string LocalConnectionString = $"mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb";

        static Lazy<MongoClient> dbClient = new Lazy<MongoClient>(() => new MongoClient(CloudConnectionString), true);

        static Lazy<Dictionary<string, IMongoDatabase>> connections = new Lazy<Dictionary<string, IMongoDatabase>>(true);

        public static IMongoDatabase GetDb(string dbName = DogStoreDbName)
        {

            IMongoDatabase res = null;
            if (!connections.Value.TryGetValue(dbName, out res))
            {
                res = dbClient.Value.GetDatabase(dbName);
                connections.Value.Add(dbName, res);
            }
            return res; ;
        }
    }
}
