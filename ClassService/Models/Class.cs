using ClassService.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassService.Models;

public enum ClassStatus { Scheduled, Active, Cancelled, Done }

public class Class
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string ClassName { get; set; } = "";
    public string ClassDescription { get; set; } = "";
    public string ClassType { get; set; } = "";
    public string InstructorId { get; set; } = "";

    [BsonRepresentation(BsonType.ObjectId)]
    public string CenterId { get; set; } = "";
    
    public ClassroomDto? Classroom { get; set; }

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public ClassStatus Status { get; set; } = ClassStatus.Scheduled;
}