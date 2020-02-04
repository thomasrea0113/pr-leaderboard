using System.Collections.Generic;

namespace Leaderboard.Models.Features
{
    public interface ITaggedEntity
    {
        ICollection<TagModel> Tags { get; set; }
    }
}