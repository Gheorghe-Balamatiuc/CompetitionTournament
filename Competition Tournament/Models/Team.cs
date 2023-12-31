﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Competition_Tournament.Models;

[Table("TEAM")]
public partial class Team
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("Award_Nr")]
    public int? AwardNr { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Motto { get; set; }

    [Column("Created_On", TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTime? CreatedOn { get; set; }

    [InverseProperty("Team1")]
    public virtual ICollection<Game> GameTeam1s { get; set; } = new List<Game>();

    [InverseProperty("Team2")]
    public virtual ICollection<Game> GameTeam2s { get; set; } = new List<Game>();

    [InverseProperty("IdTeamNavigation")]
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    [ForeignKey("TeamId")]
    [InverseProperty("Teams")]
    public virtual ICollection<Competition> Competitions { get; set; } = new List<Competition>();
}
