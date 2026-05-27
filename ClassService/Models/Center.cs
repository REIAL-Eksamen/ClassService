using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassService.Models;

public class Center
{
    [BsonId]
    [BsonElement("id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = "";
    public string Address { get; set; } = "";

    public List<Classroom> Classrooms { get; set; } = new();
    
}
public class Classroom
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ClassroomId { get; set; } = "";
    public string Name { get; set; } = "";
    public int Capacity { get; set; }
}