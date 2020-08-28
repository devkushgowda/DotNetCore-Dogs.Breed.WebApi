using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dogs.Breed.WebApi.Models
{
    public class DogModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }

        [JsonProperty(Order = 1)]
        public string Name { get; set; }

        [JsonProperty(Order = 2)]
        public List<string> Description { get; set; }

        [JsonProperty(Order = 3)]
        public List<BreedCharacteristics> BreedCharacteristics { get; set; }

        [JsonProperty(Order = 4)]
        public List<VitalStats> VitalStats { get; set; }

        [JsonProperty(Order = 5)]
        public List<MoreAbout> MoreAbout { get; set; }

        [JsonProperty(Order = 6)]
        public List<string> ImagesUrls { get; set; }

        [JsonProperty(Order = 7)]
        public string ProfileUrl { get; set; }
    }

    public class MoreAbout
    {
        [JsonProperty(Order = 1)]
        public string Title { get; set; }

        [JsonProperty(Order = 2)]

        public List<string> Information = new List<string>();
    }

    public class VitalStats
    {
        [JsonProperty(Order = 1)]
        public string Title { get; set; }

        [JsonProperty(Order = 2)]
        public string Value { get; set; }
    }

    public class BreedCharacteristics
    {
        [JsonProperty(Order = 1)]
        public string Title { get; set; }

        [JsonProperty(Order = 2)]
        public int Rating { get; set; }

        [JsonProperty(Order = 3)]
        public Dictionary<string, int> Survey { get; set; }
    }
}
