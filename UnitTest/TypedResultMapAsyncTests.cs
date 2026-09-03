using FunctionalTypes.TypedErrorResult;
using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class TypedResultMapAsync_SyncResultToTaskTests
{
    [Test]
    public async Task MapAsync_OnSuccess_RunsSelectorWithValueAndWrapsResult()
    {
        Result<int> result = new Success<int>(21);

        Result<string> mapped = await result.MapAsync(v => Task.FromResult((v * 2).ToString()));

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapAsync_OnFailure_ShortCircuitsWithSameMessage()
    {
        Result<int> result = new Failure<int>("boom");
        var selectorWasCalled = false;

        Result<string> mapped = await result.MapAsync<int, string>(v =>
        {
            selectorWasCalled = true;
            return Task.FromResult(v.ToString());
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task MapAsync_ToTypedErrorResult_OnSuccess_RunsSelectorAndWrapsValue()
    {
        Result<int> result = new Success<int>(21);

        Result<string, DomainError> mapped = await result.MapAsync<int, string, DomainError>(
            v => Task.FromResult((v * 2).ToString()),
            errorSelector: _ => DomainError.Invalid);

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapAsync_ToTypedErrorResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Result<int> result = new Failure<int>("not-found");
        var selectorWasCalled = false;

        Result<string, DomainError> mapped = await result.MapAsync<int, string, DomainError>(
            v =>
            {
                selectorWasCalled = true;
                return Task.FromResult(v.ToString());
            },
            errorSelector: msg => msg == "not-found" ? DomainError.NotFound : DomainError.Invalid);

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}

[TestFixture]
public class TypedResultMapAsync_TaskToSyncSelectorTests
{
    [Test]
    public async Task Map_OnSuccess_RunsSelector()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(21));

        Result<string> mapped = await resultTask.Map(v => (v * 2).ToString());

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("42"));
    }

    [Test]
    public async Task Map_OnFailure_ShortCircuitsWithSameMessage()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var selectorWasCalled = false;

        Result<string> mapped = await resultTask.Map<int, string>(v =>
        {
            selectorWasCalled = true;
            return v.ToString();
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task Map_ToTypedErrorResult_OnSuccess_RunsSelector()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(21));

        Result<string, DomainError> mapped = await resultTask.Map(
            v => (v * 2).ToString(),
            errorSelector: _ => DomainError.Invalid);

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("42"));
    }

    [Test]
    public async Task Map_ToTypedErrorResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var selectorWasCalled = false;

        Result<string, DomainError> mapped = await resultTask.Map<int, string, DomainError>(
            v =>
            {
                selectorWasCalled = true;
                return v.ToString();
            },
            errorSelector: _ => DomainError.Invalid);

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}

[TestFixture]
public class TypedResultMapAsync_TaskToTaskTests
{
    [Test]
    public async Task MapAsync_OnSuccess_RunsSelector()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(21));

        Result<string> mapped = await resultTask.MapAsync(v => Task.FromResult((v * 2).ToString()));

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapAsync_OnFailure_ShortCircuitsWithSameMessage()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var selectorWasCalled = false;

        Result<string> mapped = await resultTask.MapAsync<int, string>(v =>
        {
            selectorWasCalled = true;
            return Task.FromResult(v.ToString());
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task MapAsync_ToTypedErrorResult_OnSuccess_RunsSelector()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(21));

        Result<string, DomainError> mapped = await resultTask.MapAsync<int, string, DomainError>(
            v => Task.FromResult((v * 2).ToString()),
            errorSelector: _ => DomainError.Invalid);

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapAsync_ToTypedErrorResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var selectorWasCalled = false;

        Result<string, DomainError> mapped = await resultTask.MapAsync<int, string, DomainError>(
            v =>
            {
                selectorWasCalled = true;
                return Task.FromResult(v.ToString());
            },
            errorSelector: _ => DomainError.Invalid);

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}
