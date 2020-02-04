using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Leaderboards.Models
{
    // TODO implement sluggy on save
    public class LeaderboardModel : IDbEntity<LeaderboardModel>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
        public virtual ICollection<UserLeaderboard> UserLeaderboards { get; set; } = new List<UserLeaderboard>();

        public void OnModelCreating(EntityTypeBuilder<LeaderboardModel> builder)
        {
            // ensuring Name is unique
            builder.HasIndex(p => p.Name).IsUnique();
        }
    }
}