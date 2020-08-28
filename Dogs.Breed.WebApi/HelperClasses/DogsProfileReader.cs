using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dogs.Breed.WebApi.Models;
using NJson = Newtonsoft.Json;

namespace Dogs.Breed.WebApi.HelperClasses
{
    public class DogsProfileReader
    {
        static readonly string basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "DogBreedsData");


        public static IEnumerable<DogModel> GetAllProfiles(string path = null)
        {
            var dirs = Directory.GetDirectories(path ?? basePath);
            var result = dirs.Where(dir => File.Exists(Path.Combine(dir, "about.json"))).Select(dir =>
            {
                var res = NJson.JsonConvert.DeserializeObject<DogModel>(File.ReadAllText(Path.Combine(dir, "about.json")));
                res.Id = Regex.Replace(res.Name, @"\s+", "");
                return res;
            });

            return result;
        }
    }
}
