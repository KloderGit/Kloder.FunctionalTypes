using FunctionalTypes.Bridging;
using FunctionalTypes.SimpleResult;
using FunctionalTypes.TypedErrorResult;
using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class TypedErrorResultBridgeAsync_ToSimpleResultTests
{
    [Test]
    public async Task BindAsync_OnSuccess_RunsBinderAndBridgesToSimple()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);

        Result bound = await result.BindAsync(
            _ => Task.FromResult<Result>(new Success()),
            errorSelector: e => e.ToString());

        Assert.That(bound.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public async Task BindAsync_OnFailure_ShortCircuitsAndConvertsError()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);
        var binderWasCalled = false;

        Result bound = await result.BindAsync(_ =>
        {
            binderWasCalled = true;
            return Task.FromResult<Result>(new Success());
        }, errorSelector: e => e.ToString());

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(() => "success", e => e), Is.EqualTo(nameof(DomainError.NotFound)));
    }

    [Test]
    public async Task Bind_TaskToSyncBinder_OnSuccess_RunsBinder()
    {
        Task<Result<int, DomainError>> resultTask = Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42));

        Result bound = await resultTask.Bind(
            _ => new Success(),
            errorSelector: e => e.ToString());

        Assert.That(bound.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public async Task Bind_TaskToSyncBinder_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.Invalid));
        var binderWasCalled = false;

        Result bound = await resultTask.Bind(_ =>
        {
            binderWasCalled = true;
            return new Success();
        }, errorSelector: e => e.ToString());

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(() => "success", e => e), Is.EqualTo(nameof(DomainError.Invalid)));
    }

    [Test]
    public async Task BindAsync_TaskToTask_OnSuccess_RunsBinder()
    {
        Task<Result<int, DomainError>> resultTask = Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42));

        Result bound = await resultTask.BindAsync(
            _ => Task.FromResult<Result>(new Success()),
            errorSelector: e => e.ToString());

        Assert.That(bound.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public async Task BindAsync_TaskToTask_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));
        var binderWasCalled = false;

        Result bound = await resultTask.BindAsync(_ =>
        {
            binderWasCalled = true;
            return Task.FromResult<Result>(new Success());
        }, errorSelector: e => e.ToString());

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(() => "success", e => e), Is.EqualTo(nameof(DomainError.NotFound)));
    }
}

[TestFixture]
public class TypedErrorResultBridgeAsync_ToTypedResultTests
{
    [Test]
    public async Task BindAsync_OnSuccess_RunsBinderAndBridgesToTyped()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);

        Result<string> bound = await result.BindAsync(
            v => Task.FromResult<Result<string>>(new Success<string>($"value={v}")),
            errorSelector: e => e.ToString());

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public async Task BindAsync_OnFailure_ShortCircuitsAndConvertsError()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.Invalid);
        var binderWasCalled = false;

        Result<string> bound = await result.BindAsync<int, string, DomainError>(v =>
        {
            binderWasCalled = true;
            return Task.FromResult<Result<string>>(new Success<string>($"value={v}"));
        }, errorSelector: e => e.ToString());

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => "success", e => e), Is.EqualTo(nameof(DomainError.Invalid)));
    }

    [Test]
    public async Task Bind_TaskToSyncBinder_OnSuccess_RunsBinder()
    {
        Task<Result<int, DomainError>> resultTask = Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42));

        Result<string> bound = await resultTask.Bind(
            v => new Success<string>($"value={v}"),
            errorSelector: e => e.ToString());

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public async Task Bind_TaskToSyncBinder_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));
        var binderWasCalled = false;

        Result<string> bound = await resultTask.Bind<int, string, DomainError>(v =>
        {
            binderWasCalled = true;
            return new Success<string>($"value={v}");
        }, errorSelector: e => e.ToString());

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => "success", e => e), Is.EqualTo(nameof(DomainError.NotFound)));
    }

    [Test]
    public async Task BindAsync_TaskToTask_OnSuccess_RunsBinder()
    {
        Task<Result<int, DomainError>> resultTask = Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42));

        Result<string> bound = await resultTask.BindAsync<int, string, DomainError>(
            v => Task.FromResult<Result<string>>(new Success<string>($"value={v}")),
            errorSelector: e => e.ToString());

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public async Task BindAsync_TaskToTask_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.Invalid));
        var binderWasCalled = false;

        Result<string> bound = await resultTask.BindAsync<int, string, DomainError>(v =>
        {
            binderWasCalled = true;
            return Task.FromResult<Result<string>>(new Success<string>($"value={v}"));
        }, errorSelector: e => e.ToString());

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => "success", e => e), Is.EqualTo(nameof(DomainError.Invalid)));
    }
}
