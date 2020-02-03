# Leaderboard


## Unit Tests

Unit tests use a temporaty database that get's wiped every time the db-unit container starts. If you create objects in the unit test, but provide hard-coded names and indexes - then your tests will always fail on subsiquent runs. ALWAYS use a fixture to generate Keys/Indexes when testing, like so:

```csharp
[Theory, AutoData]
public async Task TestModifyProfile(string leaderboardName, string userName) => await WithScopeAsync(async scope =>
{
    var manager = scope.ServiceProvider.GetRequiredService<UserProfileManager>();
    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var profile = await AddUserAsync(manager, userName, default);
    Assert.Empty(profile.UserLeaderboards);

    var leaderboard = new LeaderboardModel {
        Name = $"leaderboard {leaderboardName}"
    };

    profile.UserLeaderboards.Add(new UserLeaderboard {
        Leaderboard = leaderboard,
        User = profile
    });


    await manager.SaveChangesAsync();
}
```

The `AutoData` attribute will ensure unique parameter values are always supplied.