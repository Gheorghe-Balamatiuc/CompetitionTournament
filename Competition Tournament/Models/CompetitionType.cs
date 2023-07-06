using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Competition_Tournament.Models;

[Table("COMPETITION_TYPE")]
public partial class CompetitionType
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Name { get; set; }

    [InverseProperty("CompetitionTypeNavigation")]
    public virtual ICollection<Competition> Competitions { get; set; } = new List<Competition>();
}
