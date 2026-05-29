using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassService.Models;

// Status fortæller, hvilken tilstand et konkret hold er i.
public enum ClassStatus
{
    Planlagt,
    Aktiv,
    Aflyst,
    Færdigt
}

// Repræsenterer et konkret hold på et bestemt tidspunkt, i et bestemt center og lokale.
public class Class
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    // Reference til den holdtemplate, som holdet er oprettet ud fra.
    [BsonRepresentation(BsonType.ObjectId)]
    public string TemplateId { get; set; } = "";

    // Reference til instruktøren fra AdminService.
    [BsonRepresentation(BsonType.ObjectId)]
    public string InstructorId { get; set; } = "";

    // Reference til centeret, hvor holdet foregår.
    [BsonRepresentation(BsonType.ObjectId)]
    public string CenterId { get; set; } = "";

    // Reference til lokalet i centeret.
    [BsonRepresentation(BsonType.ObjectId)]
    public string ClassroomId { get; set; } = "";

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    // Nye hold starter som planlagte.
    public ClassStatus Status { get; set; } = ClassStatus.Planlagt;
    
    // Liste over brugere, der er tilmeldt holdet.
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> UserIds { get; set; } = new();
}