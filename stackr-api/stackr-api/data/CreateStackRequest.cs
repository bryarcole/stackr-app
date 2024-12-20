using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System.Collections.Generic;

namespace Stackr_Api.Data
{
    public record CreateStackRequest
    {
        public string StackId { get; set; }
        public required string Title { get; init; }
        public List<KeyValuePair<string, double>> StackScores { get; init; }
        public required List<string> Stack { get; init; }
    }

    public class Stack
    {
        public string Id { get; set;}
        public string Name { get; set; }
    }
}