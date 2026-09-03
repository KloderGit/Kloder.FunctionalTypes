using FunctionalTypes.Either;

namespace UnitTest;

[TestFixture]
public class EitherMatchAsync_SyncEitherToTaskTests
{
    [Test]
    public async Task MatchAsync_OnLeft_RunsOnLeftBranch()
    {
        Either<int, string> either = Either<int, string>.Left(21);

        string outcome = await either.MatchAsync(
            onLeft: v => Task.FromResult((v * 2).ToString()),
            onRight: v => Task.FromResult(v));

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task MatchAsync_OnRight_RunsOnRightBranch()
    {
        Either<int, string> either = Either<int, string>.Right("hello");

        string outcome = await either.MatchAsync(
            onLeft: v => Task.FromResult(v.ToString()),
            onRight: v => Task.FromResult(v));

        Assert.That(outcome, Is.EqualTo("hello"));
    }
}

[TestFixture]
public class EitherMatchAsync_TaskToSyncBranchesTests
{
    [Test]
    public async Task Match_OnLeft_RunsOnLeftBranch()
    {
        Task<Either<int, string>> eitherTask = Task.FromResult(Either<int, string>.Left(21));

        string outcome = await eitherTask.Match(v => (v * 2).ToString(), v => v);

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task Match_OnRight_RunsOnRightBranch()
    {
        Task<Either<int, string>> eitherTask = Task.FromResult(Either<int, string>.Right("hello"));

        string outcome = await eitherTask.Match(v => v.ToString(), v => v);

        Assert.That(outcome, Is.EqualTo("hello"));
    }
}

[TestFixture]
public class EitherMatchAsync_TaskToTaskTests
{
    [Test]
    public async Task MatchAsync_OnLeft_RunsOnLeftBranch()
    {
        Task<Either<int, string>> eitherTask = Task.FromResult(Either<int, string>.Left(21));

        string outcome = await eitherTask.MatchAsync(
            onLeft: v => Task.FromResult((v * 2).ToString()),
            onRight: v => Task.FromResult(v));

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task MatchAsync_OnRight_RunsOnRightBranch()
    {
        Task<Either<int, string>> eitherTask = Task.FromResult(Either<int, string>.Right("hello"));

        string outcome = await eitherTask.MatchAsync(
            onLeft: v => Task.FromResult(v.ToString()),
            onRight: v => Task.FromResult(v));

        Assert.That(outcome, Is.EqualTo("hello"));
    }
}
