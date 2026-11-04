using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TallerMecanico.Api.Models;

public partial class Reparacion
{
    public static readonly string[] EstadosValidos = ["Pendiente", "EnProceso", "Finalizada"];

    [Key]
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "VehiculoId es obligatorio.")]
    public int VehiculoId { get; set; }

    [Required]
    [StringLength(200)]
    public string Descripcion { get; set; } = null!;

    [Range(0, 99999999.99, ErrorMessage = "El costo debe ser mayor o igual a 0.")]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Costo { get; set; }

    public DateOnly Fecha { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression("^(Pendiente|EnProceso|Finalizada)$", ErrorMessage = "Estado debe ser Pendiente, EnProceso o Finalizada.")]
    public string Estado { get; set; } = "Pendiente";

    [ForeignKey("VehiculoId")]
    [InverseProperty("Reparaciones")]
    public virtual Vehiculo? Vehiculo { get; set; }
}
