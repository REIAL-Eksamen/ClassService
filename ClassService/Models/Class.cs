using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassService.Models;

public enum ClassStatus
{
    Planlagt,
    Aktiv,
    Aflyst,
    Færdigt
}

public class Class
{
    [BsonId]
    [BsonElement("id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string TemplateId { get; set; } = "";    // → ClassTemplate._id

    [BsonRepresentation(BsonType.ObjectId)]
    public string InstructorId { get; set; } = "";  // → Admin._id

    [BsonRepresentation(BsonType.ObjectId)]
    public string CenterId { get; set; } = "";      // → Center._id

    [BsonRepresentation(BsonType.ObjectId)]
    public string ClassroomId { get; set; } = "";   // → Center.Classrooms[]._id

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public ClassStatus Status { get; set; } = ClassStatus.Planlagt;
}