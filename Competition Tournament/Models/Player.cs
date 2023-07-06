using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Competition_Tournament.Models;

[Table("PLAYER")]
public partial class Player
{
    [Key]
    public int Id { get; set; }

    [Column("First_Name")]
    [StringLength(255)]
    [Unicode(false)]
    public string? FirstName { get; set; }

    [Column("Last_Name")]
    [StringLength(255)]
    [Unicode(false)]
    public string? LastName { get; set; }

    public int? Age { get; set; }

    [Column("Id_Team")]
    public int? IdTeam { get; set; }

    [ForeignKey("IdTeam")]
    [InverseProperty("Players")]
    public virtual Team? IdTeamNavigation { get; set; }
}
