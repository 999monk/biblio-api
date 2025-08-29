using BIblioApi.models.DTOs;
using BIblioApi.services;
using Microsoft.AspNetCore.Mvc;

namespace BIblioApi.controllers;

[ApiController]
[Route("api/prestamos")]
public class PrestamosController : ControllerBase
{
    private readonly IPrestamoService _prestamoService;

    public PrestamosController(IPrestamoService prestamoService)
    {
        _prestamoService = prestamoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PrestamoDTO>>> GetPrestamos()
    {
        var prestamos = await _prestamoService.GetAllPrestamosAsync();
        return Ok(prestamos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PrestamoDTO>> GetPrestamo(int id)
    {
        var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
        if (prestamo == null) return NotFound();
        return Ok(prestamo);
    }

    [HttpGet("activos")]
    public async Task<ActionResult<IEnumerable<PrestamoDTO>>> GetPrestamosActivos()
    {
        var prestamos = await _prestamoService.GetPrestamosActivosAsync();
        return Ok(prestamos);
    }

    [HttpGet("socio/{socioId}")]
    public async Task<ActionResult<IEnumerable<PrestamoDTO>>> GetPrestamosPorSocio(int socioId)
    {
        var prestamos = await _prestamoService.GetPrestamosBySocioAsync(socioId);
        return Ok(prestamos);
    }

    [HttpPost]
    public async Task<ActionResult<PrestamoDTO>> PostPrestamo(RegistrarPrestamoDTO registrarPrestamoDto)
    {
        try
        {
            var nuevoPrestamo = await _prestamoService.CreatePrestamoAsync(registrarPrestamoDto);
            return CreatedAtAction(nameof(GetPrestamo), new { id = nuevoPrestamo.Id }, nuevoPrestamo);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}/devolver")]
    public async Task<IActionResult> DevolverPrestamo(int id)
    {
        var result = await _prestamoService.DevolverPrestamoAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/renovar")]
    public async Task<IActionResult> RenovarPrestamo(int id)
    {
        var result = await _prestamoService.RenovarPrestamoAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}