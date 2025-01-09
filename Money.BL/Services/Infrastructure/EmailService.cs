using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Money.BL.Interfaces.Infrastructure;
using Money.BL.Models.Email;
using Money.Common;
using Money.Common.Options;

namespace Money.BL.Services.Infrastructure;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly EmailOptions _emailOptions;
    private readonly ITemplateRenderer _templateRenderer;

    public EmailService(ILogger<EmailService> logger, IOptions<EmailOptions> emailOptions, ITemplateRenderer templateRenderer)
    {
        _logger = logger;
        _emailOptions = emailOptions.Value;
        _templateRenderer = templateRenderer;
    }

    public async Task SendAsync(string notificationEmail, EmailTemplateType templateType, EmailTemplateModel model)
    {
        var subject = templateType switch
        {
            EmailTemplateType.EmailConfirmation => "Email confirmation",
            EmailTemplateType.TwoFactorAuthToggle => "2FA confirmation",
            EmailTemplateType.EmailChange => "Email changing",
            EmailTemplateType.PasswordReset => "Password changing",
            _ => "Notification"
        };

        var htmlContent = _templateRenderer.RenderTemplate(templateType, model);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Money App", _emailOptions.SenderEmail));
        message.To.Add(new MailboxAddress("", notificationEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlContent
        };

        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_emailOptions.SmtpServer, _emailOptions.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailOptions.SenderEmail, _emailOptions.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email has been sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending email: {ex.Message}");
        }
    }
}
