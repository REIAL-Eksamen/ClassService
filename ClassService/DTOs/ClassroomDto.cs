using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassService.DTOs;

public class ClassroomDto
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ClassroomId { get; set; } = "";
    public string ClassroomName { get; set; } = "";
    public int Capacity { get; set; }
}