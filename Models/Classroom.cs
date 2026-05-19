using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassService.Models;

public class Classroom
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ClassroomId { get; set; }

    public string Name { get; set; } = "";
    public int Capacity { get; set; }

    // Classroom tilhører ét center
    [BsonRepresentation(BsonType.ObjectId)]
    public string CenterId { get; set; } = "";
}