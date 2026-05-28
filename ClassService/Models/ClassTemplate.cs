using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassService.Models;
// Beskriver en holdtype, som senere kan bruges til at oprette konkrete hold.
public class ClassTemplate
{
    // MongoDB bruger ObjectId som id, men i C# arbejder vi med det som en string
    [BsonId]
    [BsonElement("id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string ClassName { get; set; } = "";
    public string ClassDescription { get; set; } = "";
    public string ClassType { get; set; } = "";
}