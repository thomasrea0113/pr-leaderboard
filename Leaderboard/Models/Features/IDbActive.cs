namespace Leaderboard.Models.Features
{
    /// <summary>
    /// Will prevent the model from being deleted, and instead simply be set as inactive
    /// </summary>
    public interface IDbActive
    {
        bool IsActive { get; set; }
    }
}