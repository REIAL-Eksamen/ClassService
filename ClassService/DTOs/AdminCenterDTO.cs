using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassService.DTOs;

public class AdminCenterDTO
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string AdminId { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Role { get; set; } = "";
    public string CenterId { get; set; } = "";
}