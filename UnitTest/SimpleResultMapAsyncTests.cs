using FunctionalTypes.SimpleResult;
using FunctionalTypes.TypedErrorResult;
using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class SimpleResultMapAsync_SyncResultToTaskTests
{
    [Test]
    public async Task MapAsync_ToTypedResult_OnSuccess_RunsSelectorAndWrapsValue()
    {
        Result result = new Success();

        Result<int> mapped = await result.MapAsync(() => Task.FromResult(42));

        Assert.That(mapped.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task MapAsync_ToTypedResult_OnFailure_ShortCircuitsWithSameMessage()
    {
        Result result = new Failure("boom");
        var selectorWasCalled = false;

        Result<int> mapped = await result.MapAsync<int>(() =>
        {
            selectorWasCalled = true;
            return Task.FromResult(42);
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task MapAsync_ToTypedErrorResult_OnSuccess_RunsSelectorAndWrapsValue()
    {
        Result result = new Success();

        Result<int, DomainError> mapped = await result.MapAsync<int, DomainError>(
            () => Task.FromResult(42),
            errorSelector: _ => DomainError.Invalid);

        Assert.That(mapped.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task MapAsync_ToTypedErrorResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Result result = new Failure("not-found");
        var selectorWasCalled = false;

        Result<int, DomainError> mapped = await result.MapAsync<int, DomainError>(
            () =>
            {
                selectorWasCalled = true;
                return Task.FromResult(42);
            },
            errorSelector: msg => msg == "not-found" ? DomainError.NotFound : DomainError.Invalid);

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}

[TestFixture]
public class SimpleResultMapAsync_TaskToSyncSelectorTests
{
    [Test]
    public async Task Map_ToTypedResult_OnSuccess_RunsSelector()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        Result<int> mapped = await resultTask.Map(() => 42);

        Assert.That(mapped.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task Map_ToTypedResult_OnFailure_ShortCircuitsWithSameMessage()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var selectorWasCalled = false;

        Result<int> mapped = await resultTask.Map<int>(() =>
        {
            selectorWasCalled = true;
            return 42;
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task Map_ToTypedErrorResult_OnSuccess_RunsSelector()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        Result<int, DomainError> mapped = await resultTask.Map(
            () => 42,
            errorSelector: _ => DomainError.Invalid);

        Assert.That(mapped.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task Map_ToTypedErrorResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var selectorWasCalled = false;

        Result<int, DomainError> mapped = await resultTask.Map<int, DomainError>(
            () =>
            {
                selectorWasCalled = true;
                return 42;
            },
            errorSelector: _ => DomainError.Invalid);

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}

[TestFixture]
public class SimpleResultMapAsync_TaskToTaskTests
{
    [Test]
    public async Task MapAsync_ToTypedResult_OnSuccess_RunsSelector()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        Result<int> mapped = await resultTask.MapAsync(() => Task.FromResult(42));

        Assert.That(mapped.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task MapAsync_ToTypedResult_OnFailure_ShortCircuitsWithSameMessage()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var selectorWasCalled = false;

        Result<int> mapped = await resultTask.MapAsync<int>(() =>
        {
            selectorWasCalled = true;
            return Task.FromResult(42);
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task MapAsync_ToTypedErrorResult_OnSuccess_RunsSelector()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        Result<int, DomainError> mapped = await resultTask.MapAsync<int, DomainError>(
            () => Task.FromResult(42),
            errorSelector: _ => DomainError.Invalid);

        Assert.That(mapped.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task MapAsync_ToTypedErrorResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var selectorWasCalled = false;

        Result<int, DomainError> mapped = await resultTask.MapAsync<int, DomainError>(
            () =>
            {
                selectorWasCalled = true;
                return Task.FromResult(42);
            },
            errorSelector: _ => DomainError.Invalid);

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}
