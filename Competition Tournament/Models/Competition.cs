using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Competition_Tournament.Models;

[Table("COMPETITION")]
public partial class Competition
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("Start_Date", TypeName = "date")]
    public DateTime? StartDate { get; set; }

    [Column("End_Date", TypeName = "date")]
    public DateTime? EndDate { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Location { get; set; }

    [Column("Competition_Type")]
    public int? CompetitionType { get; set; }

    [ForeignKey("CompetitionType")]
    [InverseProperty("Competitions")]
    public virtual CompetitionType? CompetitionTypeNavigation { get; set; }

    [InverseProperty("Competition")]
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    [ForeignKey("CompetitionId")]
    [InverseProperty("Competitions")]
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
