using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PartyGamesByTDNG.API.DbContexts;

public partial class PartyGamesByTdngContext : DbContext
{
    public PartyGamesByTdngContext()
    {
    }

    public PartyGamesByTdngContext(DbContextOptions<PartyGamesByTdngContext> options)
        : base(options)
    {
    }

    public virtual DbSet<HubGroup> HubGroups { get; set; }

    public virtual DbSet<HubMember> HubMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HubGroup>(entity =>
        {
            entity.HasKey(e => e.RoomCode).HasName("PK_HubGroups_1");

            entity.Property(e => e.RoomCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.GameCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.LastActivity).HasColumnType("datetime");
            entity.Property(e => e.Passcode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RoomName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HubMember>(entity =>
        {
            entity.Property(e => e.ConnectionId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastActivity).HasColumnType("datetime");
            entity.Property(e => e.RoomCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
