﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Delab.Shared.Entities;

public class SoftPlan
{
    [Key]
    public int SoftPlanId { get; set; }

    [MaxLength(50, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Required(ErrorMessage = "El campo {0} es Requerido")]
    [Display(Name = "Plan Comercial")]
    public string? Name { get; set; }

    [DisplayFormat(DataFormatString = "{0:C2}")]
    [Display(Name = "Precio")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "El campo {0} es Requerido")]
    [Display(Name = "Mes(es)")]
    public int Meses { get; set; }

    [Required(ErrorMessage = "El campo {0} es Requerido")]
    [Display(Name = "Clinicas #")]
    public int ClinicsCount { get; set; }

    [Display(Name = "Activo")]
    public bool Active { get; set; }

    //Relaciones

    public ICollection<Corporation>? Corporations { get; set; }
}