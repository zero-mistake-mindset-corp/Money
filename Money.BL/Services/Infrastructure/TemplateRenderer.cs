using Money.BL.Interfaces.Infrastructure;
using Money.BL.Models.Email;
using Money.Common;

namespace Money.BL.Services.Infrastructure;

public class TemplateRenderer : ITemplateRenderer
{
    public string RenderTemplate(EmailTemplateType templateType, EmailTemplateModel model)
    {
        var templateFileName = templateType switch
        {
            EmailTemplateType.EmailConfirmation => "EmailConfirmation.html",
            EmailTemplateType.TwoFactorAuthToggle => "TwoFactorAuthToggle.html",
            EmailTemplateType.EmailChange => "EmailChange.html",
            EmailTemplateType.PasswordReset => "PasswordReset.html",
            _ => throw new ArgumentOutOfRangeException(nameof(templateType))
        };

        var filePath = Path.Combine("EmailTemplates", templateFileName);
        var templateContent = File.ReadAllText(filePath);

        templateContent = templateContent.Replace("{{UserName}}", model.UserName ?? string.Empty);
        templateContent = templateContent.Replace("{{Code}}", model.Code ?? string.Empty);
        templateContent = templateContent.Replace("{{NewEmail}}", model.NewEmail ?? string.Empty);
        templateContent = templateContent.Replace("{{ToggleAction}}", model.ToggleAction ?? string.Empty);

        return templateContent;
    }
}
