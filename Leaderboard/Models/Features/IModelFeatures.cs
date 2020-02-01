using System;

namespace Leaderboard.Models.Features
{
    [Flags]
    public enum ModelFeatures {
        PreventDelete
    }

    public interface IModelFeatures
    {
        ModelFeatures Features { get; }
    }
}