using FunctionalTypes.SimpleResult;

namespace UnitTest;

[TestFixture]
public class SimpleResultCheckAsync_SyncResultToTaskTests
{
    [Test]
    public async Task CheckAsync_OnSuccess_PredicateTrue_ReturnsUnchangedResult()
    {
        Result result = new Success();

        Result checked_ = await result.CheckAsync(() => Task.FromResult(true));

        Assert.That(checked_, Is.SameAs(result));
    }

    [Test]
    public async Task CheckAsync_OnSuccess_PredicateFalse_ReturnsFailureWithMessage()
    {
        Result result = new Success();

        Result checked_ = await result.CheckAsync(() => Task.FromResult(false), "too small");

        Assert.That(checked_.Match(() => "success", e => e), Is.EqualTo("too small"));
    }

    [Test]
    public async Task CheckAsync_OnSuccess_PredicateFalse_NoMessage_UsesDefaultMessage()
    {
        Result result = new Success();

        Result checked_ = await result.CheckAsync(() => Task.FromResult(false));

        Assert.That(checked_.Match(() => "success", e => e), Is.EqualTo("Check failed"));
    }

    [Test]
    public async Task CheckAsync_OnFailure_SkipsPredicateAndReturnsUnchangedResult()
    {
        Result result = new Failure("boom");
        var predicateWasCalled = false;

        Result checked_ = await result.CheckAsync(() =>
        {
            predicateWasCalled = true;
            return Task.FromResult(true);
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.Match(() => "success", e => e), Is.EqualTo("boom"));
    }
}

[TestFixture]
public class SimpleResultCheckAsync_TaskToSyncPredicateTests
{
    [Test]
    public async Task Check_OnSuccess_PredicateFalse_ReturnsFailureWithMessage()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        Result checked_ = await resultTask.Check(() => false, "too small");

        Assert.That(checked_.Match(() => "success", e => e), Is.EqualTo("too small"));
    }

    [Test]
    public async Task Check_OnFailure_SkipsPredicate()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var predicateWasCalled = false;

        Result checked_ = await resultTask.Check(() =>
        {
            predicateWasCalled = true;
            return true;
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.Match(() => "success", e => e), Is.EqualTo("boom"));
    }
}

[TestFixture]
public class SimpleResultCheckAsync_TaskToTaskTests
{
    [Test]
    public async Task CheckAsync_OnSuccess_PredicateFalse_ReturnsFailureWithMessage()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        Result checked_ = await resultTask.CheckAsync(() => Task.FromResult(false), "too small");

        Assert.That(checked_.Match(() => "success", e => e), Is.EqualTo("too small"));
    }

    [Test]
    public async Task CheckAsync_OnFailure_SkipsPredicate()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var predicateWasCalled = false;

        Result checked_ = await resultTask.CheckAsync(() =>
        {
            predicateWasCalled = true;
            return Task.FromResult(true);
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.Match(() => "success", e => e), Is.EqualTo("boom"));
    }
}
