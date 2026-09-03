using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class TypedResultCheckAsync_SyncResultToTaskTests
{
    [Test]
    public async Task CheckAsync_OnSuccess_PredicateTrue_ReturnsUnchangedResult()
    {
        Result<int> result = new Success<int>(42);

        Result<int> checked_ = await result.CheckAsync(v => Task.FromResult(v > 0));

        Assert.That(checked_, Is.SameAs(result));
    }

    [Test]
    public async Task CheckAsync_OnSuccess_PredicateFalse_ReturnsFailureWithMessage()
    {
        Result<int> result = new Success<int>(-1);

        Result<int> checked_ = await result.CheckAsync(v => Task.FromResult(v > 0), "must be positive");

        Assert.That(checked_.Match(_ => "success", e => e), Is.EqualTo("must be positive"));
    }

    [Test]
    public async Task CheckAsync_OnSuccess_PredicateFalse_NoMessage_UsesDefaultMessage()
    {
        Result<int> result = new Success<int>(-1);

        Result<int> checked_ = await result.CheckAsync(v => Task.FromResult(v > 0));

        Assert.That(checked_.Match(_ => "success", e => e), Is.EqualTo("Check failed"));
    }

    [Test]
    public async Task CheckAsync_OnFailure_SkipsPredicateAndReturnsUnchangedResult()
    {
        Result<int> result = new Failure<int>("boom");
        var predicateWasCalled = false;

        Result<int> checked_ = await result.CheckAsync(v =>
        {
            predicateWasCalled = true;
            return Task.FromResult(v > 0);
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }
}

[TestFixture]
public class TypedResultCheckAsync_TaskToSyncPredicateTests
{
    [Test]
    public async Task Check_OnSuccess_PredicateFalse_ReturnsFailureWithMessage()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(-1));

        Result<int> checked_ = await resultTask.Check(v => v > 0, "must be positive");

        Assert.That(checked_.Match(_ => "success", e => e), Is.EqualTo("must be positive"));
    }

    [Test]
    public async Task Check_OnFailure_SkipsPredicate()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var predicateWasCalled = false;

        Result<int> checked_ = await resultTask.Check(v =>
        {
            predicateWasCalled = true;
            return v > 0;
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }
}

[TestFixture]
public class TypedResultCheckAsync_TaskToTaskTests
{
    [Test]
    public async Task CheckAsync_OnSuccess_PredicateFalse_ReturnsFailureWithMessage()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(-1));

        Result<int> checked_ = await resultTask.CheckAsync(v => Task.FromResult(v > 0), "must be positive");

        Assert.That(checked_.Match(_ => "success", e => e), Is.EqualTo("must be positive"));
    }

    [Test]
    public async Task CheckAsync_OnFailure_SkipsPredicate()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var predicateWasCalled = false;

        Result<int> checked_ = await resultTask.CheckAsync(v =>
        {
            predicateWasCalled = true;
            return Task.FromResult(v > 0);
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }
}
