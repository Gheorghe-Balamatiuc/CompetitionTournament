using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Competition_Tournament.Models;

[Table("GAME")]
public partial class Game
{
    [Key]
    public int Id { get; set; }

    [Column("Team1_Id")]
    public int? Team1Id { get; set; }

    [Column("Team2_Id")]
    public int? Team2Id { get; set; }

    [Column("Team1_Goals")]
    public int? Team1Goals { get; set; }

    [Column("Team2_Goals")]
    public int? Team2Goals { get; set; }

    [Column("Competition_Id")]
    public int? CompetitionId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Date { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Stadium { get; set; }

    [ForeignKey("CompetitionId")]
    [InverseProperty("Games")]
    public virtual Competition? Competition { get; set; }

    [ForeignKey("Team1Id")]
    [InverseProperty("GameTeam1s")]
    public virtual Team? Team1 { get; set; }

    [ForeignKey("Team2Id")]
    [InverseProperty("GameTeam2s")]
    public virtual Team? Team2 { get; set; }
}
