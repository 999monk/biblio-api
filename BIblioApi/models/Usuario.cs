﻿namespace BIblioApi.models;

public class Usuario
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = "Usuario";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}