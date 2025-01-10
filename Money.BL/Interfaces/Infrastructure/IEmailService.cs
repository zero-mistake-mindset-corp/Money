using Money.BL.Models.Email;
using Money.Common;

namespace Money.BL.Interfaces.Infrastructure;

public interface IEmailService
{
    Task SendAsync(string notificationEmail, EmailTemplateType templateType, EmailTemplateModel model);
}
