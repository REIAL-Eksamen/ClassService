using ClassService.Models;
using ClassService.DTO;
using ClassService.Clients;
using MongoDB.Driver;

namespace ClassService.Services;

public class ClassService
{
    private readonly IMongoCollection<Class> _classes;
    private readonly ClassTemplateService _templateService;
    private readonly ClassroomService _classroomService;
    private readonly AdminClient _adminClient;
    private readonly UserClient _userClient;

    public ClassService(
        IConfiguration config,
        ClassTemplateService templateService,
        ClassroomService classroomService,
        AdminClient adminClient,
        UserClient userClient)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:Database"]);
        _classes = db.GetCollection<Class>("classes");
        _templateService = templateService;
        _classroomService = classroomService;
        _adminClient = adminClient;
        _userClient = userClient;
    }

    public Task<List<Class>> GetAllAsync() =>
        _classes.Find(_ => true).ToListAsync();

    public async Task<ClassResponse> GetWithDetailsAsync(string id)
    {
        var c = await _classes.Find(x => x.Id == id).FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException($"Hold {id} findes ikke.");

        var instructor = await _adminClient.GetInstructorAsync(c.InstructorId);
        var attendees  = await _userClient.GetUsersAsync(c.AttendeeIds);
        var waitlist   = await _userClient.GetUsersAsync(c.WaitlistIds);

        return new ClassResponse
        {
            Id               = c.Id,
            ClassName        = c.ClassName,
            ClassDescription = c.ClassDescription,
            ClassType        = c.ClassType,
            Instructor       = instructor,
            CenterId         = c.CenterId,
            ClassroomId      = c.ClassroomId,
            StartTime        = c.StartTime,
            EndTime          = c.EndTime,
            ClassCapacity    = c.ClassCapacity,
            Status           = c.Status,
            Attendees        = attendees,
            Waitlist         = waitlist
        };
    }

    public Task<List<Class>> GetByCenterAsync(string centerId) =>
        _classes.Find(x => x.CenterId == centerId).ToListAsync();

    public async Task<Class> CreateAsync(CreateClassRequest request)
    {
        var (className, classDescription, classType) = await ResolveClassFields(request);

        var classroom = await _classroomService.GetByIdAsync(request.ClassroomId)
            ?? throw new ArgumentException($"Lokale {request.ClassroomId} findes ikke.");

        if (classroom.CenterId != request.CenterId)
            throw new ArgumentException("Lokalet tilhører ikke det valgte center.");

        if (request.ClassCapacity.HasValue && request.ClassCapacity > classroom.Capacity)
            throw new ArgumentException(
                $"Holdkapacitet ({request.ClassCapacity}) overstiger lokalets kapacitet ({classroom.Capacity}).");

        var instructor = await _adminClient.GetInstructorAsync(request.InstructorId)
                         ?? throw new ArgumentException($"Instruktør {request.InstructorId} findes ikke.");

        await CheckRoomAvailabilityAsync(request.ClassroomId, request.StartTime, request.EndTime, excludeId: null);

        var newClass = new Class
        {
            ClassName        = className,
            ClassDescription = classDescription,
            ClassType        = classType,
            InstructorId     = request.InstructorId,
            CenterId         = request.CenterId,
            ClassroomId      = request.ClassroomId,
            StartTime        = request.StartTime,
            EndTime          = request.EndTime,
            ClassCapacity    = request.ClassCapacity ?? classroom.Capacity,
        };

        await _classes.InsertOneAsync(newClass);
        return newClass;
    }

    public async Task UpdateAsync(string id, CreateClassRequest request)
    {
        var existing = await _classes.Find(x => x.Id == id).FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException($"Hold {id} findes ikke.");

        var (className, classDescription, classType) = await ResolveClassFields(request);

        var classroom = await _classroomService.GetByIdAsync(request.ClassroomId)
            ?? throw new ArgumentException($"Lokale {request.ClassroomId} findes ikke.");

        if (classroom.CenterId != request.CenterId)
            throw new ArgumentException("Lokalet tilhører ikke det valgte center.");

        if (request.ClassCapacity.HasValue && request.ClassCapacity > classroom.Capacity)
            throw new ArgumentException(
                $"Holdkapacitet ({request.ClassCapacity}) overstiger lokalets kapacitet ({classroom.Capacity}).");

        var instructor = await _adminClient.GetInstructorAsync(request.InstructorId)
                         ?? throw new ArgumentException($"Instruktør {request.InstructorId} findes ikke.");

        await CheckRoomAvailabilityAsync(request.ClassroomId, request.StartTime, request.EndTime, excludeId: id);

        var updated = new Class
        {
            Id               = id,
            ClassName        = className,
            ClassDescription = classDescription,
            ClassType        = classType,
            InstructorId     = request.InstructorId,
            CenterId         = request.CenterId,
            ClassroomId      = request.ClassroomId,
            StartTime        = request.StartTime,
            EndTime          = request.EndTime,
            ClassCapacity    = request.ClassCapacity ?? classroom.Capacity,
            Status           = existing.Status,
            AttendeeIds      = existing.AttendeeIds,
            WaitlistIds      = existing.WaitlistIds
        };

        await _classes.ReplaceOneAsync(x => x.Id == id, updated);
    }

    public async Task DeleteAsync(string id)
    {
        var result = await _classes.DeleteOneAsync(x => x.Id == id);
        if (result.DeletedCount == 0)
            throw new KeyNotFoundException($"Hold {id} findes ikke.");
    }

    public async Task AddAttendeeAsync(string classId, string userId)
    {
        var trainingClass = await _classes.Find(x => x.Id == classId).FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException($"Hold {classId} findes ikke.");

        if (trainingClass.AttendeeIds.Contains(userId))
            throw new ArgumentException("Bruger er allerede tilmeldt holdet.");

        if (trainingClass.AttendeeIds.Count >= trainingClass.ClassCapacity)
        {
            var waitlistUpdate = Builders<Class>.Update.AddToSet(x => x.WaitlistIds, userId);
            await _classes.UpdateOneAsync(x => x.Id == classId, waitlistUpdate);
            return;
        }

        var update = Builders<Class>.Update.AddToSet(x => x.AttendeeIds, userId);
        await _classes.UpdateOneAsync(x => x.Id == classId, update);
    }

   public async Task RemoveAttendeeAsync(string classId, string userId)
    {
        var trainingClass = await _classes.Find(x => x.Id == classId).FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException($"Hold {classId} findes ikke.");

        var update = Builders<Class>.Update.Pull(x => x.AttendeeIds, userId);
        await _classes.UpdateOneAsync(x => x.Id == classId, update);

        if (trainingClass.WaitlistIds.Count > 0)
        {
            var nextUserId = trainingClass.WaitlistIds.First();
            var waitlistUpdate = Builders<Class>.Update
                .Pull(x => x.WaitlistIds, nextUserId)
                .AddToSet(x => x.AttendeeIds, nextUserId);
            await _classes.UpdateOneAsync(x => x.Id == classId, waitlistUpdate);
        }
    }

    private async Task<(string ClassName, string ClassDescription, string ClassType)> ResolveClassFields(
        CreateClassRequest request)
    {
        if (request.TemplateId is not null)
        {
            var template = await _templateService.GetByIdAsync(request.TemplateId)
                ?? throw new ArgumentException($"Skabelon {request.TemplateId} findes ikke.");

            return (template.ClassName, template.ClassDescription, template.ClassType);
        }

        if (string.IsNullOrEmpty(request.ClassName) ||
            string.IsNullOrEmpty(request.ClassDescription) ||
            string.IsNullOrEmpty(request.ClassType))
            throw new ArgumentException("ClassName, ClassDescription og ClassType er påkrævet uden skabelon.");

        return (request.ClassName, request.ClassDescription, request.ClassType);
    }

    private async Task CheckRoomAvailabilityAsync(
        string classroomId,
        DateTime? startTime,
        DateTime? endTime,
        string? excludeId)
    {
        var filter = Builders<Class>.Filter.And(
            Builders<Class>.Filter.Eq(x => x.ClassroomId, classroomId),
            Builders<Class>.Filter.Ne(x => x.Status, ClassStatus.Cancelled),
            Builders<Class>.Filter.Lt(x => x.StartTime, endTime),
            Builders<Class>.Filter.Gt(x => x.EndTime, startTime)
        );

        if (excludeId is not null)
            filter &= Builders<Class>.Filter.Ne(x => x.Id, excludeId);

        var conflict = await _classes.Find(filter).FirstOrDefaultAsync();

        if (conflict is not null)
            throw new ArgumentException(
                $"Lokalet er allerede booket fra {conflict.StartTime} til {conflict.EndTime}.");
    }
}