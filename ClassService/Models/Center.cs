using ClassService.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassService.Models;

public class Center
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = "";
    public string Address { get; set; } = "";

    public List<CenterAdmin> Admins { get; set; } = new();
    public List<ClassroomDto> Classrooms { get; set; } = new();
}
public class CenterAdmin
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string AdminId { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Role { get; set; } = "";
}