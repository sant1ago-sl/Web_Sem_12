using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace TallerMecanico.Api.Models;

[Index("Placa", Name = "UQ_Vehiculos_Placa", IsUnique = true)]
public partial class Vehiculo
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "La placa es obligatoria.")]
    [StringLength(10, MinimumLength = 6)]
    public string Placa { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Marca { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Modelo { get; set; } = null!;

    [Range(1950, 2100, ErrorMessage = "El anio debe estar entre 1950 y 2100.")]
    public int Anio { get; set; }

    [Required]
    [StringLength(100)]
    public string Propietario { get; set; } = null!;

    [StringLength(20)]
    public string? Telefono { get; set; }

    [JsonIgnore]
    [InverseProperty("Vehiculo")]
    public virtual ICollection<Reparacion> Reparaciones { get; set; } = new List<Reparacion>();
}
