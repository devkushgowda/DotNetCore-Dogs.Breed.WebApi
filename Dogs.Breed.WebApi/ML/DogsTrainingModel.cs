using Dogs.Breed.WebApi.HelperClasses;
using Dogs.Breed.WebApi.ML.Interfaces;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dogs.Breed.WebApi.ML
{
    public class DogTrainInput : IMlData
    {
        public string _id { set; get; }
        public string Text { set; get; }
    }

    public class DogsTrainingEngine : AbstractTrainModel<DogTrainInput, MlPredictionOutput>
    {
        private const string dataFolder = "data";
        private static string dataFolderPath = Path.Combine(Environment.CurrentDirectory, dataFolder);

        public static string ModelFilePath = Path.Combine(dataFolderPath, $"{nameof(DogsTrainingEngine).ToLower()}.zip");

        public override string ModelOutputPath => ModelFilePath;


        public override List<DogTrainInput> LoadData()
        {
            List<DogTrainInput> result = new List<DogTrainInput>();

            DogDataStore.GetProfiles().ToList()
                .ForEach(trainData =>
                {
                    result.Add(new DogTrainInput { _id = trainData.Id, Text = trainData.Name });
                    trainData.Description.ForEach(text => result.Add(new DogTrainInput { _id = trainData.Id, Text = text }));
                    trainData.MoreAbout.ForEach(moreAbouts => moreAbouts.Information.ForEach(moreAbout => result.Add(new DogTrainInput { _id = trainData.Id, Text = moreAbout })));
                    //trainData.VitalStats.ForEach(vitalStats => result.Add(new DogTrainInput { _id = trainData.Id, Text = vitalStats.Title + vitalStats.Value }));
                }
                );
            return result;
        }


        public override EstimatorChain<KeyToValueMappingTransformer> TransformAndBuildPipeline()
        {
            var fText = $"{nameof(DogTrainInput.Text)}Featurized";
            var transformedData = _mlContext.Transforms.Text.FeaturizeText(inputColumnName: nameof(DogTrainInput.Text), outputColumnName: fText)
                .Append(_mlContext.Transforms.Concatenate("Features", fText));


            if (_enableCache)
                transformedData.AppendCacheCheckpoint(_mlContext);   //Remove for large datasets.

            var processedData = _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: nameof(DogTrainInput._id), outputColumnName: "Label")
                .Append(transformedData);

            var result = processedData.Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
         .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));  //Build pipeline

            return result;
        }
    }

}
