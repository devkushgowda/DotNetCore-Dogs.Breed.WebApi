using System.Collections.Generic;
using System.IO;
using Dogs.Breed.WebApi.Models;
using NJson = Newtonsoft.Json;

namespace Dogs.Breed.WebApi.HelperClasses
{
    public class DogsProfileReader
    {
        public static IEnumerable<DogModel> GetAllProfiles(string path = null)
        {
            var result = NJson.JsonConvert.DeserializeObject<List<DogModel>>(File.ReadAllText("dogStore.json"));
            return result;
        }
    }
}
