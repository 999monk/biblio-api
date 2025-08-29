namespace BIblioApi.services;

public interface IMailService
{
    Task SendMailAsync(string toEmail, string subject, string content);
    Task EnviarMailConfirmacionPrestamo(int prestamoId);
}
