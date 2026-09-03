using FunctionalTypes.SimpleResult;

namespace UnitTest;

[TestFixture]
public class SimpleResultMatchAsync_SyncResultToTaskTests
{
    [Test]
    public async Task MatchAsync_OnSuccess_RunsSuccessBranch()
    {
        Result result = new Success();

        string outcome = await result.MatchAsync(
            success: () => Task.FromResult("ok"),
            failure: e => Task.FromResult(e));

        Assert.That(outcome, Is.EqualTo("ok"));
    }

    [Test]
    public async Task MatchAsync_OnFailure_RunsFailureBranch()
    {
        Result result = new Failure("boom");

        string outcome = await result.MatchAsync(
            success: () => Task.FromResult("ok"),
            failure: e => Task.FromResult(e));

        Assert.That(outcome, Is.EqualTo("boom"));
    }
}

[TestFixture]
public class SimpleResultMatchAsync_TaskToSyncBranchesTests
{
    [Test]
    public async Task Match_OnSuccess_RunsSuccessBranch()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        string outcome = await resultTask.Match(() => "ok", e => e);

        Assert.That(outcome, Is.EqualTo("ok"));
    }

    [Test]
    public async Task Match_OnFailure_RunsFailureBranch()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));

        string outcome = await resultTask.Match(() => "ok", e => e);

        Assert.That(outcome, Is.EqualTo("boom"));
    }
}

[TestFixture]
public class SimpleResultMatchAsync_TaskToTaskTests
{
    [Test]
    public async Task MatchAsync_OnSuccess_RunsSuccessBranch()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        string outcome = await resultTask.MatchAsync(
            success: () => Task.FromResult("ok"),
            failure: e => Task.FromResult(e));

        Assert.That(outcome, Is.EqualTo("ok"));
    }

    [Test]
    public async Task MatchAsync_OnFailure_RunsFailureBranch()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));

        string outcome = await resultTask.MatchAsync(
            success: () => Task.FromResult("ok"),
            failure: e => Task.FromResult(e));

        Assert.That(outcome, Is.EqualTo("boom"));
    }
}
