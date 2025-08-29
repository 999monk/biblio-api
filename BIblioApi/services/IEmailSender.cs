using MimeKit;

namespace BIblioApi.services;

public interface IEmailSender
{
    Task SendAsync(MimeMessage message);
}
