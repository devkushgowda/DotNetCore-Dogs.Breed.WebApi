using System.Linq;
using Dogs.Breed.WebApi.HelperClasses;
using Dogs.Breed.WebApi.ML;
using Dogs.Breed.WebApi.ML.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using MongoDB.Driver;

namespace Dogs.Breed.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DogStoreController : ControllerBase
    {
        private readonly ILogger<DogStoreController> _logger;
        private readonly PredictionEnginePool<DogTrainInput, MlPredictionOutput> _predictionEnginePool;
        public DogStoreController(ILogger<DogStoreController> logger, PredictionEnginePool<DogTrainInput, MlPredictionOutput> predictionEnginePool)
        {
            _logger = logger;
            _predictionEnginePool = predictionEnginePool;
        }

        [HttpGet]
        public object GetProfiles()
        {
            var res = DogDataStore.GetProfiles();
            return Ok(new { Count = res.Count(), Profiles = res });
        }

        [HttpGet]
        [Route("ml/{input}")]
        public object MlFind(string input)
        {
            var id = _predictionEnginePool.Predict(nameof(DogsPredictionEngine), new DogTrainInput { Text = input })._id;
            var res = DogDataStore.ProfilebyId(id);
            if (res != null)
                return Ok(res);
            else
                return Ok($"No prediction for input : '{input}' .");
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

        //[HttpGet]
        //[Route("load")]
        //public ActionResult LoadToDatabase()
        //{
        //    var res = DogDataStore.LoadToDatabase();
        //    return Ok($"Loaded {res.Count()} records to database.");
        //}

        [HttpGet]
        [Route("update")]
        public ActionResult UpdateDataStoe()
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
