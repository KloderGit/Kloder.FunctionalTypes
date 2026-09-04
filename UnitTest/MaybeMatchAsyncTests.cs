using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeMatchAsync_SyncMaybeToTaskTests
{
    [Test]
    public async Task MatchAsync_OnJust_RunsJustBranch()
    {
        Maybe<int> maybe = Maybe<int>.Just(42);

        string outcome = await maybe.MatchAsync(
            just: v => Task.FromResult(v.ToString()),
            nothing: () => Task.FromResult("nothing"));

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task MatchAsync_OnNothing_RunsNothingBranch()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();

        string outcome = await maybe.MatchAsync(
            just: v => Task.FromResult(v.ToString()),
            nothing: () => Task.FromResult("nothing"));

        Assert.That(outcome, Is.EqualTo("nothing"));
    }
}

[TestFixture]
public class MaybeMatchAsync_TaskToSyncBranchesTests
{
    [Test]
    public async Task Match_OnJust_RunsJustBranch()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Just(42));

        string outcome = await maybeTask.Match(v => v.ToString(), () => "nothing");

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task Match_OnNothing_RunsNothingBranch()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Nothing());

        string outcome = await maybeTask.Match(v => v.ToString(), () => "nothing");

        Assert.That(outcome, Is.EqualTo("nothing"));
    }
}

[TestFixture]
public class MaybeMatchAsync_TaskToTaskTests
{
    [Test]
    public async Task MatchAsync_OnJust_RunsJustBranch()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Just(42));

        string outcome = await maybeTask.MatchAsync(
            just: v => Task.FromResult(v.ToString()),
            nothing: () => Task.FromResult("nothing"));

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task MatchAsync_OnNothing_RunsNothingBranch()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Nothing());

        string outcome = await maybeTask.MatchAsync(
            just: v => Task.FromResult(v.ToString()),
            nothing: () => Task.FromResult("nothing"));

        Assert.That(outcome, Is.EqualTo("nothing"));
    }
}
