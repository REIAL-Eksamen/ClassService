using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassService.DTOs;

public class AdminCenterDto
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string AdminId { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Role { get; set; } = "";
}