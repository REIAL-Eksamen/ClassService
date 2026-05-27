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
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string TemplateId { get; set; } = "";

    [BsonRepresentation(BsonType.ObjectId)]
    public string InstructorId { get; set; } = "";

    [BsonRepresentation(BsonType.ObjectId)]
    public string CenterId { get; set; } = "";

    [BsonRepresentation(BsonType.ObjectId)]
    public string ClassroomId { get; set; } = "";

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public ClassStatus Status { get; set; } = ClassStatus.Planlagt;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> UserIds { get; set; } = new();
}