using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class TypedResultMatchAsync_SyncResultToTaskTests
{
    [Test]
    public async Task MatchAsync_OnSuccess_RunsSuccessBranchWithValue()
    {
        Result<int> result = new Success<int>(42);

        string outcome = await result.MatchAsync(
            success: v => Task.FromResult(v.ToString()),
            failure: e => Task.FromResult(e));

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task MatchAsync_OnFailure_RunsFailureBranch()
    {
        Result<int> result = new Failure<int>("boom");

        string outcome = await result.MatchAsync(
            success: v => Task.FromResult(v.ToString()),
            failure: e => Task.FromResult(e));

        Assert.That(outcome, Is.EqualTo("boom"));
    }

    [Test]
    public async Task MatchAsync_IgnoringValue_OnSuccess_RunsSuccessBranch()
    {
        Result<int> result = new Success<int>(42);

        string outcome = await result.MatchAsync(
            success: () => Task.FromResult("ok"),
            failure: e => Task.FromResult(e));

        Assert.That(outcome, Is.EqualTo("ok"));
    }

    [Test]
    public async Task MatchAsync_IgnoringValue_OnFailure_RunsFailureBranch()
    {
        Result<int> result = new Failure<int>("boom");

        string outcome = await result.MatchAsync(
            success: () => Task.FromResult("ok"),
            failure: e => Task.FromResult(e));

        Assert.That(outcome, Is.EqualTo("boom"));
    }
}

[TestFixture]
public class TypedResultMatchAsync_TaskToSyncBranchesTests
{
    [Test]
    public async Task Match_OnSuccess_RunsSuccessBranchWithValue()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));

        string outcome = await resultTask.Match(v => v.ToString(), e => e);

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task Match_IgnoringValue_OnFailure_RunsFailureBranch()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));

        string outcome = await resultTask.Match(() => "ok", e => e);

        Assert.That(outcome, Is.EqualTo("boom"));
    }
}

[TestFixture]
public class TypedResultMatchAsync_TaskToTaskTests
{
    [Test]
    public async Task MatchAsync_OnSuccess_RunsSuccessBranchWithValue()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));

        string outcome = await resultTask.MatchAsync(
            success: v => Task.FromResult(v.ToString()),
            failure: e => Task.FromResult(e));

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task MatchAsync_IgnoringValue_OnFailure_RunsFailureBranch()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));

        string outcome = await resultTask.MatchAsync(
            success: () => Task.FromResult("ok"),
            failure: e => Task.FromResult(e));

        Assert.That(outcome, Is.EqualTo("boom"));
    }
}
