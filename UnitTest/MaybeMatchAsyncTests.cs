using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeMatchAsync_SyncMaybeToTaskTests
{
    [Test]
    public async Task MatchAsync_OnSome_RunsSomeBranch()
    {
        Maybe<int> maybe = Maybe<int>.Some(42);

        string outcome = await maybe.MatchAsync(
            some: v => Task.FromResult(v.ToString()),
            none: () => Task.FromResult("none"));

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task MatchAsync_OnNone_RunsNoneBranch()
    {
        Maybe<int> maybe = Maybe<int>.None();

        string outcome = await maybe.MatchAsync(
            some: v => Task.FromResult(v.ToString()),
            none: () => Task.FromResult("none"));

        Assert.That(outcome, Is.EqualTo("none"));
    }
}

[TestFixture]
public class MaybeMatchAsync_TaskToSyncBranchesTests
{
    [Test]
    public async Task Match_OnSome_RunsSomeBranch()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Some(42));

        string outcome = await maybeTask.Match(v => v.ToString(), () => "none");

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task Match_OnNone_RunsNoneBranch()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.None());

        string outcome = await maybeTask.Match(v => v.ToString(), () => "none");

        Assert.That(outcome, Is.EqualTo("none"));
    }
}

[TestFixture]
public class MaybeMatchAsync_TaskToTaskTests
{
    [Test]
    public async Task MatchAsync_OnSome_RunsSomeBranch()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Some(42));

        string outcome = await maybeTask.MatchAsync(
            some: v => Task.FromResult(v.ToString()),
            none: () => Task.FromResult("none"));

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task MatchAsync_OnNone_RunsNoneBranch()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.None());

        string outcome = await maybeTask.MatchAsync(
            some: v => Task.FromResult(v.ToString()),
            none: () => Task.FromResult("none"));

        Assert.That(outcome, Is.EqualTo("none"));
    }
}
