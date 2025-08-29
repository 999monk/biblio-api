using BIblioApi.models.DTOs;
using BIblioApi.services;
using Microsoft.AspNetCore.Mvc;

namespace BIblioApi.controllers;

[ApiController]
[Route("api/libros")]
public class LibrosController : ControllerBase
{
    private readonly ILibroService _libroService;

    public LibrosController(ILibroService libroService)
    {
        _libroService = libroService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LibroDTO>>> GetLibros()
    {
        var libros = await _libroService.GetAllLibrosAsync();
        return Ok(libros);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LibroDTO>> GetLibro(int id)
    {
        var libro = await _libroService.GetLibroByIdAsync(id);
        if (libro == null) return NotFound();
        
        return Ok(libro);
    }

    [HttpPost]
    public async Task<ActionResult<LibroDTO>> PostLibro(CreateLibroDTO createLibroDto)
    {
        var nuevoLibro = await _libroService.CreateLibroAsync(createLibroDto);
        return CreatedAtAction(nameof(GetLibro), new { id = nuevoLibro.Id }, nuevoLibro);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutLibro(int id, UpdateLibroDTO updateLibroDto)
    {
        var libroActualizado = await _libroService.UpdateLibroAsync(id, updateLibroDto);
        if (libroActualizado == null) return NotFound();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLibro(int id)
    {
        var result = await _libroService.DeleteLibroAsync(id);
        if (!result) return NotFound();
        
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<LibroDTO>>> SearchLibros([FromQuery] string term)
    {
        var libros = await _libroService.SearchLibrosAsync(term);
        return Ok(libros);
    }
}
