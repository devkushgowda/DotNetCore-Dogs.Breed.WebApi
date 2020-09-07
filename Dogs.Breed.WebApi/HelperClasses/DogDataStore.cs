
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dogs.Breed.WebApi.Database;
using Dogs.Breed.WebApi.Models;
using MongoDB.Bson.IO;
using Newtonsoft.Json;

namespace Dogs.Breed.WebApi.HelperClasses
{
    public class DogDataStore
    {
        public const string fileStoreName = "dogStore.json";
        static Lazy<List<DogModel>> store = new Lazy<List<DogModel>>(() => LoadFromFileOrDatabase(), true);

        private static List<DogModel> LoadFromFileOrDatabase()
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<DogModel>>(File.ReadAllText(fileStoreName));
            }
            catch
            {
                return LoadFromDatabase().ToList();
            }

        }

        public static IEnumerable<DogModel> GetProfiles() => store.Value;

        public static IEnumerable<DogModel> GetProfilesWithRange(int low, int high)
        {
            var data = store.Value;
            high = high >= data.Count ? data.Count : high;
            low = low >= data.Count ? data.Count - 1 : low;

            return data.GetRange(low, high - low);
        }



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


        public static void WriteToFileStore()
        {
            var dogProfiles = store.Value;
            File.WriteAllText(fileStoreName, Newtonsoft.Json.JsonConvert.SerializeObject(dogProfiles, Formatting.Indented));
        }

        public static IEnumerable<DogModel> UpdateStoreData()
        {
            store.Value.Clear();
            store.Value.AddRange(LoadFromDatabase());
            return store.Value;
        }

        private static IEnumerable<DogModel> LoadFromDatabase()
        {
            var db = MongoDbConnectionStore.GetDb();
            var collection = db.GetCollection<DogModel>(MongoDbConnectionStore.ProfilesCollectionName);
            var res = collection.Find(item => true).ToList();
            File.WriteAllText(fileStoreName, Newtonsoft.Json.JsonConvert.SerializeObject(res, Formatting.Indented));
            return res;
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
