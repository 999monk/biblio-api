using BIblioApi.models.DTOs;
using BIblioApi.services;
using Microsoft.AspNetCore.Mvc;

namespace BIblioApi.controllers;

[ApiController]
[Route("api/socios")]
public class SociosController : ControllerBase
{
    private readonly ISociosService _sociosService;

    public SociosController(ISociosService sociosService)
    {
        _sociosService = sociosService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SocioDTO>>> GetSocios()
    {
        var socios = await _sociosService.GetAllSociosAsync();
        return Ok(socios);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SocioDTO>> GetSocio(int id)
    {
        var socio = await _sociosService.GetSocioByIdAsync(id);
        if (socio == null) return NotFound();
        
        return Ok(socio);
    }

    [HttpPost]
    public async Task<ActionResult<SocioDTO>> PostSocio(CreateSocioDTO createSocioDto)
    {
        var nuevoSocio = await _sociosService.CreateSocioAsync(createSocioDto);
        return CreatedAtAction(nameof(GetSocio), new { id = nuevoSocio.Id }, nuevoSocio);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSocio(int id, UpdateSocioDTO updateSocioDto)
    {
        var socioActualizado = await _sociosService.UpdateSocioAsync(id, updateSocioDto);
        if (socioActualizado == null) return NotFound();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSocio(int id)
    {
        var result = await _sociosService.DeleteSocioAsync(id);
        
        if (!result) return NotFound(); 
        
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<SocioDTO>>> SearchSocios([FromQuery] string term)
    {
        var socios = await _sociosService.SearchSocioAsync(term);
        return Ok(socios);
    }
}