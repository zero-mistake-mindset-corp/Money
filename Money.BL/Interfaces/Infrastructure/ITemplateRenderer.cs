using Money.BL.Models.Email;
using Money.Common;

namespace Money.BL.Interfaces.Infrastructure;

public interface ITemplateRenderer
{
    string RenderTemplate(EmailTemplateType templateType, EmailTemplateModel model);
}
