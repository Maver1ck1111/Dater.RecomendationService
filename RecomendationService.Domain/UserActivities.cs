using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationService.Domain
{
    public class UserActivities
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid UserActivitiesId { get; set; }

        [BsonElement("LikedUsers")]
        [BsonRepresentation(BsonType.String)]
        public List<Guid> LikedUsers { get; set; } = new List<Guid>();

        [BsonElement("DislikedUsers")]
        [BsonRepresentation(BsonType.String)]
        public List<Guid> DislikedUsers { get; set; } = new List<Guid>();

        [BsonElement("LikesFromUsers")]
        [BsonRepresentation(BsonType.String)]
        public List<Guid> LikesFromUsers { get; set; } = new List<Guid>();
    }
}
