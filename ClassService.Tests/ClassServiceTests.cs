using ClassService.DTOs;
using ClassService.Models;
using ClassService.Repositories;
using ClassService.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClassService.Tests;

[TestClass]
public class ClassesServiceTests
{
    private Mock<IClassRepository> _classesMock;
    private Mock<IClassTemplateRepository> _templatesMock;
    private Mock<IClassroomRepository> _classroomsMock;
    private ClassesService _sut;

    [TestInitialize]
    public void Setup()
    {
        _classesMock = new Mock<IClassRepository>();
        _templatesMock = new Mock<IClassTemplateRepository>();
        _classroomsMock = new Mock<IClassroomRepository>();
        _sut = new ClassesService(_classesMock.Object, _templatesMock.Object, _classroomsMock.Object);
    }

    // ──────────────────────────────────────────────
    // GetAllAsync
    // ──────────────────────────────────────────────

    [TestMethod]
    public async Task GetAllAsync_ReturnsList()
    {
        var expected = new List<Class> { new Class { Id = "1", ClassName = "Yoga" } };
        _classesMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var result = await _sut.GetAllAsync();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Yoga", result[0].ClassName);
    }

    // ──────────────────────────────────────────────
    // GetByIdAsync
    // ──────────────────────────────────────────────

    [TestMethod]
    public async Task GetByIdAsync_WhenExists_ReturnsClass()
    {
        var expected = new Class { Id = "abc", ClassName = "Pilates" };
        _classesMock.Setup(r => r.GetByIdAsync("abc")).ReturnsAsync(expected);

        var result = await _sut.GetByIdAsync("abc");

        Assert.IsNotNull(result);
        Assert.AreEqual("Pilates", result.ClassName);
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        _classesMock.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Class?)null);

        var result = await _sut.GetByIdAsync("mangler");

        Assert.IsNull(result);
    }

    // ──────────────────────────────────────────────
    // GetByCenterAsync
    // ──────────────────────────────────────────────

    [TestMethod]
    public async Task GetByCenterAsync_ReturnsClassesForCenter()
    {
        var centerId = "center1";
        var expected = new List<Class> { new Class { CenterId = centerId } };
        _classesMock.Setup(r => r.GetByCenterAsync(centerId)).ReturnsAsync(expected);

        var result = await _sut.GetByCenterAsync(centerId);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(centerId, result[0].CenterId);
    }

    // ──────────────────────────────────────────────
    // CreateFromTemplateAsync
    // ──────────────────────────────────────────────

    [TestMethod]
    public async Task CreateFromTemplateAsync_ValidDto_InsertsAndReturnsClass()
    {
        var dto = BuildValidCreateClassDTO();
        var template = BuildTemplate(dto.ClassTemplateId);
        var classroom = new Classroom();

        _templatesMock.Setup(r => r.GetByIdAsync(dto.ClassTemplateId)).ReturnsAsync(template);
        _classroomsMock.Setup(r => r.GetByIdAndCenterAsync(dto.Classroom.ClassroomId, dto.CenterId))
                       .ReturnsAsync(classroom);
        _classesMock.Setup(r => r.InsertAsync(It.IsAny<Class>())).Returns(Task.CompletedTask);

        var result = await _sut.CreateFromTemplateAsync(dto);

        Assert.IsNotNull(result);
        Assert.AreEqual(template.ClassName, result.ClassName);
        Assert.AreEqual(template.ClassDescription, result.ClassDescription);
        Assert.AreEqual(template.ClassType, result.ClassType);
        Assert.AreEqual(dto.InstructorId, result.InstructorId);
        Assert.AreEqual(ClassStatus.Scheduled, result.Status);
        _classesMock.Verify(r => r.InsertAsync(It.IsAny<Class>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateFromTemplateAsync_NullClassroom_ThrowsArgumentException()
    {
        var dto = BuildValidCreateClassDTO();
        dto.Classroom = null;

        await Assert.ThrowsExactlyAsync<ArgumentException>(
            () => _sut.CreateFromTemplateAsync(dto));
    }

    [TestMethod]
    public async Task CreateFromTemplateAsync_TemplateNotFound_ThrowsKeyNotFoundException()
    {
        var dto = BuildValidCreateClassDTO();
        _templatesMock.Setup(r => r.GetByIdAsync(dto.ClassTemplateId)).ReturnsAsync((ClassTemplate?)null);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _sut.CreateFromTemplateAsync(dto));
    }

    [TestMethod]
    public async Task CreateFromTemplateAsync_ClassroomNotInCenter_ThrowsKeyNotFoundException()
    {
        var dto = BuildValidCreateClassDTO();
        var template = BuildTemplate(dto.ClassTemplateId);

        _templatesMock.Setup(r => r.GetByIdAsync(dto.ClassTemplateId)).ReturnsAsync(template);
        _classroomsMock.Setup(r => r.GetByIdAndCenterAsync(dto.Classroom.ClassroomId, dto.CenterId))
                       .ReturnsAsync((Classroom?)null);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _sut.CreateFromTemplateAsync(dto));
    }

    // ──────────────────────────────────────────────
    // UpdateAsync
    // ──────────────────────────────────────────────

    [TestMethod]
    public async Task UpdateAsync_ValidDto_UpdatesAndCallsReplace()
    {
        var id = "class1";
        var dto = BuildValidCreateClassDTO();
        var existing = new Class { Id = id, ClassName = "Gammel" };
        var template = BuildTemplate(dto.ClassTemplateId);
        var classroom = new Classroom();

        _classesMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
        _templatesMock.Setup(r => r.GetByIdAsync(dto.ClassTemplateId)).ReturnsAsync(template);
        _classroomsMock.Setup(r => r.GetByIdAndCenterAsync(dto.Classroom.ClassroomId, dto.CenterId))
                       .ReturnsAsync(classroom);
        _classesMock.Setup(r => r.ReplaceAsync(id, It.IsAny<Class>())).Returns(Task.CompletedTask);

        await _sut.UpdateAsync(id, dto);

        Assert.AreEqual(template.ClassName, existing.ClassName);
        _classesMock.Verify(r => r.ReplaceAsync(id, existing), Times.Once);
    }

    [TestMethod]
    public async Task UpdateAsync_NullClassroom_ThrowsArgumentException()
    {
        var dto = BuildValidCreateClassDTO();
        dto.Classroom = null;

        await Assert.ThrowsExactlyAsync<ArgumentException>(
            () => _sut.UpdateAsync("id", dto));
    }

    [TestMethod]
    public async Task UpdateAsync_ClassNotFound_ThrowsKeyNotFoundException()
    {
        var dto = BuildValidCreateClassDTO();
        _classesMock.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Class?)null);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _sut.UpdateAsync("mangler", dto));
    }

    [TestMethod]
    public async Task UpdateAsync_TemplateNotFound_ThrowsKeyNotFoundException()
    {
        var id = "class1";
        var dto = BuildValidCreateClassDTO();
        _classesMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new Class { Id = id });
        _templatesMock.Setup(r => r.GetByIdAsync(dto.ClassTemplateId)).ReturnsAsync((ClassTemplate?)null);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _sut.UpdateAsync(id, dto));
    }

    [TestMethod]
    public async Task UpdateAsync_ClassroomNotInCenter_ThrowsKeyNotFoundException()
    {
        var id = "class1";
        var dto = BuildValidCreateClassDTO();
        var template = BuildTemplate(dto.ClassTemplateId);

        _classesMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new Class { Id = id });
        _templatesMock.Setup(r => r.GetByIdAsync(dto.ClassTemplateId)).ReturnsAsync(template);
        _classroomsMock.Setup(r => r.GetByIdAndCenterAsync(dto.Classroom.ClassroomId, dto.CenterId))
                       .ReturnsAsync((Classroom?)null);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _sut.UpdateAsync(id, dto));
    }

    // ──────────────────────────────────────────────
    // DeleteAsync
    // ──────────────────────────────────────────────

    [TestMethod]
    public async Task DeleteAsync_WhenExists_CallsDelete()
    {
        _classesMock.Setup(r => r.DeleteAsync("class1")).ReturnsAsync(true);

        await _sut.DeleteAsync("class1");

        _classesMock.Verify(r => r.DeleteAsync("class1"), Times.Once);
    }

    [TestMethod]
    public async Task DeleteAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        _classesMock.Setup(r => r.DeleteAsync(It.IsAny<string>())).ReturnsAsync(false);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _sut.DeleteAsync("mangler"));
    }

    // ──────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────

    private static CreateClassDTO BuildValidCreateClassDTO() => new CreateClassDTO
    {
        ClassTemplateId = "template1",
        InstructorId = "instructor1",
        CenterId = "center1",
        Classroom = new ClassroomDto { ClassroomId = "room1" },
        StartTime = DateTime.UtcNow,
        EndTime = DateTime.UtcNow.AddHours(1)
    };

    private static ClassTemplate BuildTemplate(string id) => new ClassTemplate
    {
        Id = id,
        ClassName = "Yoga",
        ClassDescription = "Afslappende yoga",
        ClassType = "Wellness"
    };
}