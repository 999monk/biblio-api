using System.Threading.Tasks;
using BIblioApi.controllers;
using BIblioApi.models.DTOs;
using BIblioApi.services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BIblioApi.Tests.Controllers;

public class SociosControllerTests
{
    private readonly Mock<ISociosService> _mockSociosService;
    private readonly SociosController _controller;

    public SociosControllerTests()
    {
        _mockSociosService = new Mock<ISociosService>();
        _controller = new SociosController(_mockSociosService.Object);
    }

    [Fact]
    public async Task GetSocio_ShouldReturnOk_WhenSocioExists()
    {
        // Arrange
        var socioId = 1;
        var socioDto = new SocioDTO { Id = socioId, Name = "Socio de Prueba", Code = "S001", Email = "test@test.com" };
        _mockSociosService.Setup(s => s.GetSocioByIdAsync(socioId)).ReturnsAsync(socioDto);

        // Act
        var result = await _controller.GetSocio(socioId);

        // Assert
        result.Should().BeOfType<ActionResult<SocioDTO>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSocio = okResult.Value.Should().BeOfType<SocioDTO>().Subject;
        returnedSocio.Should().BeEquivalentTo(socioDto);
    }

    [Fact]
    public async Task GetSocio_ShouldReturnNotFound_WhenSocioDoesNotExist()
    {
        // Arrange
        var socioId = 99;
        _mockSociosService.Setup(s => s.GetSocioByIdAsync(socioId)).ReturnsAsync((SocioDTO)null);

        // Act
        var result = await _controller.GetSocio(socioId);

        // Assert
        result.Should().BeOfType<ActionResult<SocioDTO>>();
        result.Result.Should().BeOfType<NotFoundResult>();
    }
}