using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassService.Models;

//Repræsenterer et træningscenter med adresse og tilhørende lokaler.
public class Center
{
    [BsonId]
    [BsonElement("id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    // Grundoplysninger om centeret.
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";

    // Lokalerne i centeret, fx holdsal eller spinningrum.
    public List<Classroom> Classrooms { get; set; } = new();
    
}
// Repræsenterer et lokale inde i et center.
public class Classroom
{
    // Id for det enkelte lokale.
    [BsonRepresentation(BsonType.ObjectId)]
    public string ClassroomId { get; set; } = "";
    public string Name { get; set; } = "";
    public int Capacity { get; set; }
}