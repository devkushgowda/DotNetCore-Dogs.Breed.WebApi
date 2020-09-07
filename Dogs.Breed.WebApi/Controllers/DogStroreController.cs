using System;
using System.IO;
using System.Linq;
using System.Text;
using Dogs.Breed.WebApi.HelperClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Dogs.Breed.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DogStoreController : ControllerBase
    {
        private readonly ILogger<DogStoreController> _logger;

        public DogStoreController(ILogger<DogStoreController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("download/{range}")]
        public object DownloadProfiles(string range)
        {

            Tuple<int, int> parsedRange;
            if (ParseRange(range, out parsedRange))
            {
                var res = DogDataStore.GetProfilesWithRange(parsedRange.Item1, parsedRange.Item2);
                var mimeType = "application/json";
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(res, Formatting.Indented)));
                return new FileStreamResult(stream, mimeType)
                {
                    FileDownloadName = DogDataStore.fileStoreName
                };
            }
            else
            {
                return Ok($"Invalid range specification.\n" +
                          $"Usage:\n" +
                          $" get/10 - Gets from 0-10\n" +
                          $" get/10-20 - Gets from 10-20");
            }
        }

        [HttpGet]
        [Route("get/{range}")]
        public object GetProfiles(string range)
        {
            Tuple<int, int> parsedRange;
            if (ParseRange(range, out parsedRange))
            {
                var res = DogDataStore.GetProfilesWithRange(parsedRange.Item1, parsedRange.Item2);
                return Ok(new { Count = res.Count(), Profiles = res });
            }
            else
            {
                return Ok($"Invalid range specification.\n" +
                          $"Usage:\n" +
                          $" get/10 - Gets from 0-10\n" +
                          $" get/10-20 - Gets from 10-20");
            }
        }

        private static bool ParseRange(string range, out Tuple<int, int> res)
        {
            res = null;
            //res = new Tuple<int, int>(0, 0);
            var valid = false;
            if (range == null)
                return valid;
            if (range.Contains("-"))
            {
                var parts = range.Split("-");
                if (parts.Length == 2)
                {

                    if (int.TryParse(parts[0], out int val1) && int.TryParse(parts[1], out int val2) && val1 >= 0 && val1 <= val2)
                    {
                        valid = true;
                        res = new Tuple<int, int>(val1, val2);
                    }

                }
            }
            else
            {
                if (int.TryParse(range, out int val1) && val1 > 0)
                {
                    valid = true;
                    res = new Tuple<int, int>(0, val1);
                }
            }

            return valid;
        }

        [HttpGet]
        [Route("profile/{id}")]
        public object ProfilebyId(string id)
        {
            var res = DogDataStore.ProfilebyId(id);
            if (res != null)
                return Ok(res);
            else
                return Ok($"No profile found with id '{id}' .");
        }

        [HttpGet]
        [Route("search/{key}")]
        public ActionResult Search(string key)
        {
            var res = DogDataStore.Search(key);
            if (res != null)
                return Ok(new { Count = res.Count(), Records = res });
            else
                return Ok($"No record found for search '{key}' .");
        }

        [HttpGet]
        [Route("load")]
        public ActionResult LoadToDatabase()
        {
            var res = DogDataStore.LoadToDatabase();
            return Ok($"Loaded {res.Count()} records to database.");
        }

        [HttpGet]
        [Route("write")]
        public ActionResult WriteToFile()
        {
            DogDataStore.WriteToFileStore();
            return Ok("Success!");
        }

        [HttpGet]
        [Route("update")]
        public ActionResult UpdateDataStore()
        {
            var res = DogDataStore.UpdateStoreData();
            return Ok($"Updated {res.Count()} records from database.");
        }


        //[HttpGet]
        //[Route("delete")]
        //public ActionResult DropDatabase()
        //{
        //    DogDataStore.DeleteAllProfiles();
        //    return Ok($"Cleared all the records in Store and Database.");
        //}

        //// Add input data
        //var input = new ModelInput();

        //// Load model and predict output of sample data
        //ModelOutput result = ConsumeModel.Predict(input);
    }
}
