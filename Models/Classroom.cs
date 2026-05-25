using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassService.Models;

public class Classroom
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string ClassroomName { get; set; } = "";
    public int Capacity { get; set; }

    // Classroom tilhører ét center
    //[BsonRepresentation(BsonType.ObjectId)]
    public string CenterId { get; set; } = "";
}