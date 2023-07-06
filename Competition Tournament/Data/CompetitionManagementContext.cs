using System;
using System.Collections.Generic;
using Competition_Tournament.Models;
using Microsoft.EntityFrameworkCore;

namespace Competition_Tournament.Data;

public partial class CompetitionManagementContext : DbContext
{
    public CompetitionManagementContext()
    {
    }

    public CompetitionManagementContext(DbContextOptions<CompetitionManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<CompetitionType> CompetitionTypes { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-J23D4K1; Initial catalog=CompetitionManagement; trusted_connection=yes; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Competition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__COMPETIT__3214EC07FB846642");

            entity.HasOne(d => d.CompetitionTypeNavigation).WithMany(p => p.Competitions).HasConstraintName("FK__COMPETITI__Compe__3E52440B");

            entity.HasMany(d => d.Teams).WithMany(p => p.Competitions)
                .UsingEntity<Dictionary<string, object>>(
                    "TeamCompetition",
                    r => r.HasOne<Team>().WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TEAM_COMP__Team___49C3F6B7"),
                    l => l.HasOne<Competition>().WithMany()
                        .HasForeignKey("CompetitionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TEAM_COMP__Compe__48CFD27E"),
                    j =>
                    {
                        j.HasKey("CompetitionId", "TeamId");
                        j.ToTable("TEAM_COMPETITION");
                        j.IndexerProperty<int>("CompetitionId").HasColumnName("Competition_Id");
                        j.IndexerProperty<int>("TeamId").HasColumnName("Team_Id");
                    });
        });

        modelBuilder.Entity<CompetitionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TYPE__3214EC0787090B2C");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GAME__3214EC07DCF17A32");

            entity.HasOne(d => d.Competition).WithMany(p => p.Games).HasConstraintName("FK__GAME__Competitio__4F7CD00D");

            entity.HasOne(d => d.Team1).WithMany(p => p.GameTeam1s).HasConstraintName("FK__GAME__Team1_Id__4D94879B");

            entity.HasOne(d => d.Team2).WithMany(p => p.GameTeam2s).HasConstraintName("FK__GAME__Team2_Id__4E88ABD4");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PLAYER__3214EC077548A4DD");

            entity.HasOne(d => d.IdTeamNavigation).WithMany(p => p.Players).HasConstraintName("FK__PLAYER__Id_Team__398D8EEE");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TEAM__3214EC078C6DC96E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
