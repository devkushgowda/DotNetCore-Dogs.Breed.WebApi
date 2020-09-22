using Dogs.Breed.WebApi.ML.Interfaces;

namespace Dogs.Breed.WebApi.ML
{
    public class DogsPredictionEngine : AbstractPredictModel<DogTrainInput, MlPredictionOutput>
    {
        public static string ModelFilePath => DogsTrainingEngine.ModelFilePath;

        private string _path;

        public DogsPredictionEngine() : this(null, false)
        {

        }

        public DogsPredictionEngine(string path = null, bool initilize = false)
        {
            _path = path ?? ModelFilePath;
            if (initilize)
                Initilize();
        }
        public override string ModelOutputPath => _path;
    }
}
