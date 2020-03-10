using System;

namespace Leaderboard.Models.Features
{
    public interface ISlugged
    {
        string Name { get; set; }
        string Slug { get; set; }
    }
}
