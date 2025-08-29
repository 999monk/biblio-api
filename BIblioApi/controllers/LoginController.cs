
using BIblioApi.models.DTOs;
using BIblioApi.services;
using Microsoft.AspNetCore.Mvc;

namespace BIblioApi.controllers;

[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;

    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO loginDto)
    {
        var token = await _loginService.LoginAsync(loginDto);

        if (token == null) return Unauthorized("Credenciales inválidas.");

        return Ok(new { Token = token });
    }
}
