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
        public required List<string> Stack { get; init; }
    }

    public class Stack
    {
        [BsonId]
        public string StackId { get; set;}

        public List<KeyValuePair<string, double>> StackScores { get; set; }

        public required List<string> Ranks { get; init; }

        [BsonElement("createdAt")] // Optional: Stores the creation timestamp
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")] // Optional: Stores the last update timestamp
        public DateTime? UpdatedAt { get; set; }

    }
}