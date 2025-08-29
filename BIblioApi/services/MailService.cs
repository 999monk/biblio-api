using BIblioApi.Configuration;
using BIblioApi.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BIblioApi.services;

public class MailService : IMailService
{
    private readonly DataContext _context;
    private readonly IEmailSender _emailSender;
    private readonly MailSettings _mailSettings;

    public MailService(DataContext context, IEmailSender emailSender, IOptions<MailSettings> mailSettings)
    {
        _context = context;
        _emailSender = emailSender;
        _mailSettings = mailSettings.Value;
    }

    public async Task SendMailAsync(string toEmail, string subject, string content)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = content };
        email.Body = builder.ToMessageBody();

        await _emailSender.SendAsync(email);
    }

    public async Task EnviarMailConfirmacionPrestamo(int prestamoId)
    {
        var prestamo = await _context.Prestamos
            .Include(p => p.Socio)
            .Include(p => p.Libro)
            .FirstOrDefaultAsync(p => p.Id == prestamoId);

        if (prestamo == null || string.IsNullOrEmpty(prestamo.Socio.Email)) return;

        var destinatario = prestamo.Socio.Email;
        var asunto = "Confirmación de Préstamo de Libro";
        var cuerpo = $"<h1>Confirmación de Préstamo</h1>"
                     + $"<p>Hola {prestamo.Socio.Name},</p>"
                     + $"<p>Te confirmamos el préstamo del libro <strong>'{prestamo.Libro.Title}'</strong>.</p>"
                     + $"<p>Debes devolverlo antes del <strong>{prestamo.ReturnDate:dd/MM/yyyy}</strong>.</p>"
                     + $"<p>¡Gracias por usar BiblioAPI!</p>";

        await SendMailAsync(destinatario, asunto, cuerpo);
    }
}