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
        public Guid UserActivitiesId { get; set; }

        [BsonElement("LikedUsers")]
        List<Guid> LikedUsers { get; set; } = new List<Guid>();

        [BsonElement("DislikedUsers")]
        List<Guid> DislikedUsers { get; set; } = new List<Guid>();
    }
}
