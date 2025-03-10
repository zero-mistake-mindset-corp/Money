using System.ComponentModel.DataAnnotations;

namespace Money.BL.Models.Type;

public class CreateIncomeTypeModel
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name max length is 100 symbols")]
    public string Name { get; set; }
}
