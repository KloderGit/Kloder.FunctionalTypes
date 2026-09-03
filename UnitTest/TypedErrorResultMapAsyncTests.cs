using FunctionalTypes.TypedErrorResult;
using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class TypedErrorResultMapAsync_SyncResultToTaskTests
{
    [Test]
    public async Task MapAsync_OnSuccess_RunsSelectorWithValueAndWrapsResult()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(21);

        Result<string, DomainError> mapped = await result.MapAsync(v => Task.FromResult((v * 2).ToString()));

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapAsync_OnFailure_ShortCircuitsWithSameError()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);
        var selectorWasCalled = false;

        Result<string, DomainError> mapped = await result.MapAsync<int, string, DomainError>(v =>
        {
            selectorWasCalled = true;
            return Task.FromResult(v.ToString());
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }

    [Test]
    public async Task MapAsync_ToTypedResult_OnSuccess_RunsSelectorAndWrapsValue()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(21);

        Result<string> mapped = await result.MapAsync(
            v => Task.FromResult((v * 2).ToString()),
            errorSelector: e => e.ToString());

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapAsync_ToTypedResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);
        var selectorWasCalled = false;

        Result<string> mapped = await result.MapAsync<int, string, DomainError>(
            v =>
            {
                selectorWasCalled = true;
                return Task.FromResult(v.ToString());
            },
            errorSelector: e => e.ToString());

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => "success", e => e), Is.EqualTo(nameof(DomainError.NotFound)));
    }
}

[TestFixture]
public class TypedErrorResultMapAsync_TaskToSyncSelectorTests
{
    [Test]
    public async Task Map_OnSuccess_RunsSelector()
    {
        Task<Result<int, DomainError>> resultTask = Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(21));

        Result<string, DomainError> mapped = await resultTask.Map(v => (v * 2).ToString());

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("42"));
    }

    [Test]
    public async Task Map_OnFailure_ShortCircuitsWithSameError()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));
        var selectorWasCalled = false;

        Result<string, DomainError> mapped = await resultTask.Map<int, string, DomainError>(v =>
        {
            selectorWasCalled = true;
            return v.ToString();
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }

    [Test]
    public async Task Map_ToTypedResult_OnSuccess_RunsSelector()
    {
        Task<Result<int, DomainError>> resultTask = Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(21));

        Result<string> mapped = await resultTask.Map(
            v => (v * 2).ToString(),
            errorSelector: e => e.ToString());

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("42"));
    }

    [Test]
    public async Task Map_ToTypedResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));
        var selectorWasCalled = false;

        Result<string> mapped = await resultTask.Map<int, string, DomainError>(
            v =>
            {
                selectorWasCalled = true;
                return v.ToString();
            },
            errorSelector: e => e.ToString());

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => "success", e => e), Is.EqualTo(nameof(DomainError.NotFound)));
    }
}

[TestFixture]
public class TypedErrorResultMapAsync_TaskToTaskTests
{
    [Test]
    public async Task MapAsync_OnSuccess_RunsSelector()
    {
        Task<Result<int, DomainError>> resultTask = Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(21));

        Result<string, DomainError> mapped = await resultTask.MapAsync(v => Task.FromResult((v * 2).ToString()));

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapAsync_OnFailure_ShortCircuitsWithSameError()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));
        var selectorWasCalled = false;

        Result<string, DomainError> mapped = await resultTask.MapAsync<int, string, DomainError>(v =>
        {
            selectorWasCalled = true;
            return Task.FromResult(v.ToString());
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }

    [Test]
    public async Task MapAsync_ToTypedResult_OnSuccess_RunsSelector()
    {
        Task<Result<int, DomainError>> resultTask = Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(21));

        Result<string> mapped = await resultTask.MapAsync<int, string, DomainError>(
            v => Task.FromResult((v * 2).ToString()),
            errorSelector: e => e.ToString());

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapAsync_ToTypedResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));
        var selectorWasCalled = false;

        Result<string> mapped = await resultTask.MapAsync<int, string, DomainError>(
            v =>
            {
                selectorWasCalled = true;
                return Task.FromResult(v.ToString());
            },
            errorSelector: e => e.ToString());

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(_ => "success", e => e), Is.EqualTo(nameof(DomainError.NotFound)));
    }
}
