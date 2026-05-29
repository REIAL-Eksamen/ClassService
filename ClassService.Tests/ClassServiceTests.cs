using ClassService.Clients;
using ClassService.DTOs;
using ClassService.Models;
using ClassService.Repositories;
using ClassService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClassService.Tests;

[TestClass]
public class ClassesServiceTests
{
    private Mock<IClassRepository> _classesMock;
    private Mock<IClassTemplateRepository> _templatesMock;
    private Mock<ICenterRepository> _centersMock;
    private Mock<IAdminClient> _adminClientMock;
    private ClassesService _sut;

    [TestInitialize]
    public void Setup()
    {
        _classesMock = new Mock<IClassRepository>();
        _templatesMock = new Mock<IClassTemplateRepository>();
        _centersMock = new Mock<ICenterRepository>();
        _adminClientMock = new Mock<IAdminClient>();
        _sut = new ClassesService(
            _classesMock.Object,
            _templatesMock.Object,
            _centersMock.Object,
            _adminClientMock.Object);
    }

    // ──────────────────────────────────────────────
    // GetAllAsync
    // ──────────────────────────────────────────────

    [TestMethod]
    public async Task GetAllAsync_ReturnsList()
    {
        var expected = new List<Class> { new Class { Id = "1" } };
        _classesMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var result = await _sut.GetAllAsync();

        Assert.AreEqual(1, result.Count);
    }

    // ──────────────────────────────────────────────
    // GetByIdAsync
    // ──────────────────────────────────────────────

    [TestMethod]
    public async Task GetByIdAsync_WhenExists_ReturnsClass()
    {
        var expected = new Class { Id = "abc" };
        _classesMock.Setup(r => r.GetByIdAsync("abc")).ReturnsAsync(expected);

        var result = await _sut.GetByIdAsync("abc");

        Assert.IsNotNull(result);
        Assert.AreEqual("abc", result.Id);
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
        SetupValidMocks(dto);
        _classesMock.Setup(r => r.InsertAsync(It.IsAny<Class>())).Returns(Task.CompletedTask);

        var result = await _sut.CreateFromTemplateAsync(dto);

        Assert.IsNotNull(result);
        Assert.AreEqual(dto.TemplateId, result.TemplateId);
        Assert.AreEqual(dto.InstructorId, result.InstructorId);
        Assert.AreEqual(dto.CenterId, result.CenterId);
        Assert.AreEqual(dto.ClassroomId, result.ClassroomId);
        Assert.AreEqual(ClassStatus.Planlagt, result.Status);
        _classesMock.Verify(r => r.InsertAsync(It.IsAny<Class>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateFromTemplateAsync_TemplateNotFound_ThrowsKeyNotFoundException()
    {
        var dto = BuildValidCreateClassDTO();
        _templatesMock.Setup(r => r.GetByIdAsync(dto.TemplateId)).ReturnsAsync((ClassTemplate?)null);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _sut.CreateFromTemplateAsync(dto));
    }

    [TestMethod]
    public async Task CreateFromTemplateAsync_CenterNotFound_ThrowsKeyNotFoundException()
    {
        var dto = BuildValidCreateClassDTO();
        _templatesMock.Setup(r => r.GetByIdAsync(dto.TemplateId)).ReturnsAsync(BuildTemplate(dto.TemplateId));
        _centersMock.Setup(r => r.GetByIdAsync(dto.CenterId)).ReturnsAsync((Center?)null);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _sut.CreateFromTemplateAsync(dto));
    }

    [TestMethod]
    public async Task CreateFromTemplateAsync_ClassroomNotInCenter_ThrowsKeyNotFoundException()
    {
        var dto = BuildValidCreateClassDTO();
        _templatesMock.Setup(r => r.GetByIdAsync(dto.TemplateId)).ReturnsAsync(BuildTemplate(dto.TemplateId));
        _centersMock.Setup(r => r.GetByIdAsync(dto.CenterId)).ReturnsAsync(new Center { Id = dto.CenterId, Classrooms = new() });

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _sut.CreateFromTemplateAsync(dto));
    }

    [TestMethod]
    public async Task CreateFromTemplateAsync_AdminNotFound_ThrowsKeyNotFoundException()
    {
        var dto = BuildValidCreateClassDTO();
        _templatesMock.Setup(r => r.GetByIdAsync(dto.TemplateId)).ReturnsAsync(BuildTemplate(dto.TemplateId));
        _centersMock.Setup(r => r.GetByIdAsync(dto.CenterId)).ReturnsAsync(BuildCenter(dto.CenterId, dto.ClassroomId));
        _adminClientMock.Setup(r => r.GetAdminAsync(dto.InstructorId)).ReturnsAsync((AdminCenterDTO?)null);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _sut.CreateFromTemplateAsync(dto));
    }

    [TestMethod]
    public async Task CreateFromTemplateAsync_AdminWrongCenter_ThrowsBadHttpRequestException()
    {
        var dto = BuildValidCreateClassDTO();
        SetupValidMocks(dto, adminCenterId: "et-andet-center");

        await Assert.ThrowsExactlyAsync<BadHttpRequestException>(
            () => _sut.CreateFromTemplateAsync(dto));
    }

    [TestMethod]
    public async Task CreateFromTemplateAsync_AdminWrongRole_ThrowsBadHttpRequestException()
    {
        var dto = BuildValidCreateClassDTO();
        SetupValidMocks(dto, adminRole: "Receptionist");

        await Assert.ThrowsExactlyAsync<BadHttpRequestException>(
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
        var existing = new Class { Id = id };

        _classesMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
        SetupValidMocks(dto);
        _classesMock.Setup(r => r.ReplaceAsync(id, It.IsAny<Class>())).Returns(Task.CompletedTask);

        await _sut.UpdateAsync(id, dto);

        Assert.AreEqual(dto.TemplateId, existing.TemplateId);
        Assert.AreEqual(dto.ClassroomId, existing.ClassroomId);
        _classesMock.Verify(r => r.ReplaceAsync(id, existing), Times.Once);
    }

    [TestMethod]
    public async Task UpdateAsync_ClassNotFound_ThrowsKeyNotFoundException()
    {
        var dto = BuildValidCreateClassDTO();
        _classesMock.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Class?)null);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _sut.UpdateAsync("mangler", dto));
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
        TemplateId = "template1",
        InstructorId = "instructor1",
        CenterId = "center1",
        ClassroomId = "room1",
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

    private static Center BuildCenter(string centerId, string classroomId) => new Center
    {
        Id = centerId,
        Classrooms = new List<Classroom>
        {
            new Classroom { ClassroomId = classroomId, Name = "Sal 1", Capacity = 20 }
        }
    };

    private void SetupValidMocks(CreateClassDTO dto, string? adminCenterId = null, string adminRole = "Instruktør")
    {
        _templatesMock.Setup(r => r.GetByIdAsync(dto.TemplateId)).ReturnsAsync(BuildTemplate(dto.TemplateId));
        _centersMock.Setup(r => r.GetByIdAsync(dto.CenterId)).ReturnsAsync(BuildCenter(dto.CenterId, dto.ClassroomId));
        _adminClientMock.Setup(r => r.GetAdminAsync(dto.InstructorId)).ReturnsAsync(new AdminCenterDTO
        {
            AdminId = dto.InstructorId,
            CenterId = adminCenterId ?? dto.CenterId,
            Role = adminRole
        });
    }
}