using System;

namespace Leaderboard.Models.Features
{
    [Flags]
    public enum ModelFeatures {
    }

    public interface IModelFeatures
    {
        ModelFeatures Features { get; }
    }
}