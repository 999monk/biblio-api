using BIblioApi.models.DTOs;

namespace BIblioApi.services;

public interface ILoginService
{
    Task<string> LoginAsync(LoginDTO login);
}