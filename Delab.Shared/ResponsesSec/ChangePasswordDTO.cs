using System.ComponentModel.DataAnnotations;

namespace Delab.Shared.ResponsesSec;

public class ChangePasswordDTO
{
    [Required(ErrorMessage = "Este Campo es obligatorio")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "La Clave debe tener un minimo de {2} y un maximo de {1}")]
    [Display(Name = "Clave Actual")]
    public string CurrentPassword { get; set; } = null!;

    [Required(ErrorMessage = "Este Campo es obligatorio")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "La Clave debe tener un minimo de {2} y un maximo de {1}")]
    [Display(Name = "Nueva Clave Hebert")]
    public string NewPassword { get; set; } = null!;

    [Compare("NewPassword", ErrorMessage = "La Nueva Clave y la Confirmacion no coinciden")]
    [Required(ErrorMessage = "Este Campo es obligatorio")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "La Clave debe tener un minimo de {2} y un maximo de {1}")]
    [Display(Name = "Confirmar Clave Para Hebert")]
    public string Confirm { get; set; } = null!;
}