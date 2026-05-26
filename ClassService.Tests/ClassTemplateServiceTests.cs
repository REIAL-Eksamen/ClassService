using ClassService.DTOs;
using ClassService.Models;
using ClassService.Repositories;
using ClassService.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClassService.Tests;

[TestClass]
public class ClassTemplateServiceTests
{
    private Mock<IClassTemplateRepository> _templatesMock;
    private ClassTemplateService _sut;

    [TestInitialize]
    public void Setup()
    {
        _templatesMock = new Mock<IClassTemplateRepository>();
        _sut = new ClassTemplateService(_templatesMock.Object);
    }

    // ──────────────────────────────────────────────
    // GetAllAsync
    // ──────────────────────────────────────────────

    [TestMethod]
    public async Task GetAllAsync_ReturnsList()
    {
        var expected = new List<ClassTemplate>
        {
            new ClassTemplate { Id = "1", ClassName = "Yoga" },
            new ClassTemplate { Id = "2", ClassName = "Spinning" }
        };
        _templatesMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var result = await _sut.GetAllAsync();

        Assert.AreEqual(2, result.Count);
    }

    // ──────────────────────────────────────────────
    // GetByIdAsync
    // ──────────────────────────────────────────────

    [TestMethod]
    public async Task GetByIdAsync_WhenExists_ReturnsTemplate()
    {
        var expected = new ClassTemplate { Id = "abc", ClassName = "Pilates" };
        _templatesMock.Setup(r => r.GetByIdAsync("abc")).ReturnsAsync(expected);

        var result = await _sut.GetByIdAsync("abc");

        Assert.IsNotNull(result);
        Assert.AreEqual("Pilates", result.ClassName);
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        _templatesMock.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((ClassTemplate?)null);

        var result = await _sut.GetByIdAsync("mangler");

        Assert.IsNull(result);
    }

    // ──────────────────────────────────────────────
    // CreateAsync
    // ──────────────────────────────────────────────

    [TestMethod]
    public async Task CreateAsync_CallsInsertOnce()
    {
        var template = new ClassTemplate { ClassName = "Ny klasse" };
        _templatesMock.Setup(r => r.InsertAsync(template)).Returns(Task.CompletedTask);

        await _sut.CreateAsync(template);

        _templatesMock.Verify(r => r.InsertAsync(template), Times.Once);
    }

    // ──────────────────────────────────────────────
    // UpdateAsync
    // ──────────────────────────────────────────────

    [TestMethod]
    public async Task UpdateAsync_WhenExists_UpdatesFieldsAndCallsReplace()
    {
        var id = "template1";
        var existing = new ClassTemplate { Id = id, ClassName = "Gammel", ClassDescription = "Gammel beskrivelse", ClassType = "GammelType" };
        var dto = new CreateClassTemplateDTO { ClassName = "Ny", ClassDescription = "Ny beskrivelse", ClassType = "NyType" };

        _templatesMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
        _templatesMock.Setup(r => r.ReplaceAsync(id, existing)).Returns(Task.CompletedTask);

        await _sut.UpdateAsync(id, dto);

        Assert.AreEqual("Ny", existing.ClassName);
        Assert.AreEqual("Ny beskrivelse", existing.ClassDescription);
        Assert.AreEqual("NyType", existing.ClassType);
        _templatesMock.Verify(r => r.ReplaceAsync(id, existing), Times.Once);
    }

    [TestMethod]
    public async Task UpdateAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        _templatesMock.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((ClassTemplate?)null);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _sut.UpdateAsync("mangler", new CreateClassTemplateDTO()));
    }

    // ──────────────────────────────────────────────
    // DeleteAsync
    // ──────────────────────────────────────────────

    [TestMethod]
    public async Task DeleteAsync_WhenExists_CallsDeleteOnce()
    {
        _templatesMock.Setup(r => r.DeleteAsync("template1")).ReturnsAsync(true);

        await _sut.DeleteAsync("template1");

        _templatesMock.Verify(r => r.DeleteAsync("template1"), Times.Once);
    }

    [TestMethod]
    public async Task DeleteAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        _templatesMock.Setup(r => r.DeleteAsync(It.IsAny<string>())).ReturnsAsync(false);

        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _sut.DeleteAsync("mangler"));
    }
}