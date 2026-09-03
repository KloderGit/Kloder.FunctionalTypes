using FunctionalTypes.TypedErrorResult;

namespace UnitTest;

[TestFixture]
public class TypedErrorResultMatchAsync_SyncResultToTaskTests
{
    [Test]
    public async Task MatchAsync_OnSuccess_RunsSuccessBranchWithValue()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);

        string outcome = await result.MatchAsync(
            success: v => Task.FromResult(v.ToString()),
            failure: e => Task.FromResult(e.ToString()));

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task MatchAsync_OnFailure_RunsFailureBranchWithError()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);

        string outcome = await result.MatchAsync(
            success: v => Task.FromResult(v.ToString()),
            failure: e => Task.FromResult(e.ToString()));

        Assert.That(outcome, Is.EqualTo(nameof(DomainError.NotFound)));
    }

    [Test]
    public async Task MatchAsync_IgnoringValue_OnSuccess_RunsSuccessBranch()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);

        string outcome = await result.MatchAsync(
            success: () => Task.FromResult("ok"),
            failure: e => Task.FromResult(e.ToString()));

        Assert.That(outcome, Is.EqualTo("ok"));
    }

    [Test]
    public async Task MatchAsync_IgnoringValue_OnFailure_RunsFailureBranch()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);

        string outcome = await result.MatchAsync(
            success: () => Task.FromResult("ok"),
            failure: e => Task.FromResult(e.ToString()));

        Assert.That(outcome, Is.EqualTo(nameof(DomainError.NotFound)));
    }
}

[TestFixture]
public class TypedErrorResultMatchAsync_TaskToSyncBranchesTests
{
    [Test]
    public async Task Match_OnSuccess_RunsSuccessBranchWithValue()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42));

        string outcome = await resultTask.Match(v => v.ToString(), e => e.ToString());

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task Match_IgnoringValue_OnFailure_RunsFailureBranch()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));

        string outcome = await resultTask.Match(() => "ok", e => e.ToString());

        Assert.That(outcome, Is.EqualTo(nameof(DomainError.NotFound)));
    }
}

[TestFixture]
public class TypedErrorResultMatchAsync_TaskToTaskTests
{
    [Test]
    public async Task MatchAsync_OnSuccess_RunsSuccessBranchWithValue()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42));

        string outcome = await resultTask.MatchAsync(
            success: v => Task.FromResult(v.ToString()),
            failure: e => Task.FromResult(e.ToString()));

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public async Task MatchAsync_IgnoringValue_OnFailure_RunsFailureBranch()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));

        string outcome = await resultTask.MatchAsync(
            success: () => Task.FromResult("ok"),
            failure: e => Task.FromResult(e.ToString()));

        Assert.That(outcome, Is.EqualTo(nameof(DomainError.NotFound)));
    }
}
