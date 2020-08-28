
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dogs.Breed.WebApi.Database;
using Dogs.Breed.WebApi.Models;

namespace Dogs.Breed.WebApi.HelperClasses
{
    public class DogDataStore
    {
        static Lazy<List<DogModel>> store = new Lazy<List<DogModel>>(() => LoadFromDatabase(), true);

        private static List<DogModel> LoadFromDatabase()
        {
            var db = MongoDbConnectionStore.GetDb();
            var collection = db.GetCollection<DogModel>(MongoDbConnectionStore.ProfilesCollectionName);
            return collection.Find(item => true).ToList();
        }

        public static IEnumerable<DogModel> GetProfiles() => store.Value;

        public static DogModel ProfilebyId(string id) => store.Value.FirstOrDefault(dog => id == dog.Id);

        public static IEnumerable<String> GetImages(string id) => store.Value.FirstOrDefault(dog => id == dog.Id).ImagesUrls;

        public static IEnumerable<object> Search(string key) => store.Value
            .Where(item => !string.IsNullOrWhiteSpace(key) && item.Name.ToLower().Contains(key.ToLower()))
            .Select(item => new { Id = item.Id, Name = item.Name, ProfileImage = item.ImagesUrls.FirstOrDefault() });

        public static IEnumerable<DogModel> LoadToDatabase()
        {
            var db = MongoDbConnectionStore.GetDb();
            var dogProfiles = DogsProfileReader.GetAllProfiles();
            var collection = db.GetCollection<DogModel>(MongoDbConnectionStore.ProfilesCollectionName);
            collection.DeleteMany(_ => true);
            collection.InsertManyAsync(dogProfiles, new MongoDB.Driver.InsertManyOptions { IsOrdered = true });

            //Update stoe with latest data
            store.Value.Clear();
            store.Value.AddRange(dogProfiles);

            return dogProfiles;
        }

        public static IEnumerable<DogModel> UpdateStoreData()
        {
            store.Value.Clear();
            store.Value.AddRange(LoadFromDatabase());
            return store.Value;
        }

        public static void DeleteAllProfiles()
        {
            var db = MongoDbConnectionStore.GetDb();
            var collection = db.GetCollection<DogModel>(MongoDbConnectionStore.ProfilesCollectionName);
            var res = collection.DeleteMany(_ => true);
            store.Value.Clear();
        }
    }
}
