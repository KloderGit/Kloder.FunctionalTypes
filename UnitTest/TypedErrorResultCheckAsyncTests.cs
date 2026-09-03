using FunctionalTypes.TypedErrorResult;

namespace UnitTest;

[TestFixture]
public class TypedErrorResultCheckAsync_SyncResultToTaskTests
{
    [Test]
    public async Task CheckAsync_OnSuccess_PredicateTrue_ReturnsUnchangedResult()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);

        Result<int, DomainError> checked_ = await result.CheckAsync(
            v => Task.FromResult(v > 0),
            errorFactory: () => DomainError.Invalid);

        Assert.That(checked_, Is.SameAs(result));
    }

    [Test]
    public async Task CheckAsync_OnSuccess_PredicateFalse_ReturnsFailureFromErrorFactory()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(-1);

        Result<int, DomainError> checked_ = await result.CheckAsync(
            v => Task.FromResult(v > 0),
            errorFactory: () => DomainError.Invalid);

        Assert.That(checked_.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }

    [Test]
    public async Task CheckAsync_OnFailure_SkipsPredicateAndErrorFactory()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);
        var predicateWasCalled = false;
        var errorFactoryWasCalled = false;

        Result<int, DomainError> checked_ = await result.CheckAsync(
            v =>
            {
                predicateWasCalled = true;
                return Task.FromResult(v > 0);
            },
            errorFactory: () =>
            {
                errorFactoryWasCalled = true;
                return DomainError.Invalid;
            });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(errorFactoryWasCalled, Is.False);
        Assert.That(checked_.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}

[TestFixture]
public class TypedErrorResultCheckAsync_TaskToSyncPredicateTests
{
    [Test]
    public async Task Check_OnSuccess_PredicateFalse_ReturnsFailureFromErrorFactory()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(-1));

        Result<int, DomainError> checked_ = await resultTask.Check(
            v => v > 0,
            errorFactory: () => DomainError.Invalid);

        Assert.That(checked_.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }

    [Test]
    public async Task Check_OnFailure_SkipsPredicate()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));
        var predicateWasCalled = false;

        Result<int, DomainError> checked_ = await resultTask.Check(
            v =>
            {
                predicateWasCalled = true;
                return v > 0;
            },
            errorFactory: () => DomainError.Invalid);

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}

[TestFixture]
public class TypedErrorResultCheckAsync_TaskToTaskTests
{
    [Test]
    public async Task CheckAsync_OnSuccess_PredicateFalse_ReturnsFailureFromErrorFactory()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(-1));

        Result<int, DomainError> checked_ = await resultTask.CheckAsync(
            v => Task.FromResult(v > 0),
            errorFactory: () => DomainError.Invalid);

        Assert.That(checked_.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }

    [Test]
    public async Task CheckAsync_OnFailure_SkipsPredicate()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));
        var predicateWasCalled = false;

        Result<int, DomainError> checked_ = await resultTask.CheckAsync(
            v =>
            {
                predicateWasCalled = true;
                return Task.FromResult(v > 0);
            },
            errorFactory: () => DomainError.Invalid);

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}
