using Leaderboard.Models.Features;
using Leaderboard.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models.Relationships
{
    /// <summary>
    /// Used to declare a many-to-many relationship. Because of the way the fluent api
    /// expects relationships, we can't excplicity declare the relationship columns here.
    /// They must be declared in the base class. See <see cref="UserLeaderboard" /> for
    /// example property naming scheme.
    /// </summary>
    /// <typeparam name="TModel1">Model 1 (class name MUST be prepended with 'Model')</typeparam>
    /// <typeparam name="TModel12">Model 2 (class name MUST be prepended with 'Model')</typeparam>
    public abstract class AbstractRelationship<TModel1, TModel12>
        where TModel1 : class, new()
        where TModel12 : class, new()
    {
    }
}