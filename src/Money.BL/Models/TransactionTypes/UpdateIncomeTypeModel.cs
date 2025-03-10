using System.ComponentModel.DataAnnotations;

namespace Money.BL.Models.TransactionTypes;

public class UpdateIncomeTypeModel
{
    [Required(ErrorMessage = "Id is required")]
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name max length is 100 symbols")]
    public string Name { get; set; }
}
