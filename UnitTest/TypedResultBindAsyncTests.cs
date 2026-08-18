using FunctionalTypes.SimpleResult;
using FunctionalTypes.TypedErrorResult;
using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class TypedResultBindAsync_SyncResultToTaskTests
{
    [Test]
    public async Task BindAsync_ToTypedResult_OnSuccess_RunsBinderWithValue()
    {
        Result<int> result = new Success<int>(42);

        Result<string> bound = await result.BindAsync(v => Task.FromResult<Result<string>>(new Success<string>($"value={v}")));

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public async Task BindAsync_ToTypedResult_OnFailure_ShortCircuitsWithSameMessage()
    {
        Result<int> result = new Failure<int>("boom");
        var binderWasCalled = false;

        Result<string> bound = await result.BindAsync<int, string>(v =>
        {
            binderWasCalled = true;
            return Task.FromResult<Result<string>>(new Success<string>($"value={v}"));
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task BindAsync_ToResult_OnSuccess_RunsBinder()
    {
        Result<int> result = new Success<int>(42);

        Result bound = await result.BindAsync(() => Task.FromResult<Result>(new Success()));

        Assert.That(bound.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public async Task BindAsync_ToResult_OnFailure_ShortCircuitsWithSameMessage()
    {
        Result<int> result = new Failure<int>("boom");
        var binderWasCalled = false;

        Result bound = await result.BindAsync(() =>
        {
            binderWasCalled = true;
            return Task.FromResult<Result>(new Success());
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(() => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task BindAsync_ToTypedErrorResult_OnSuccess_RunsBinderWithValue()
    {
        Result<int> result = new Success<int>(42);

        Result<string, DomainError> bound = await result.BindAsync<int, string, DomainError>(
            v => Task.FromResult<Result<string, DomainError>>(new Success<string, DomainError>($"value={v}")),
            errorSelector: _ => DomainError.Invalid);

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public async Task BindAsync_ToTypedErrorResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Result<int> result = new Failure<int>("boom");
        var binderWasCalled = false;

        Result<string, DomainError> bound = await result.BindAsync<int, string, DomainError>(
            v =>
            {
                binderWasCalled = true;
                return Task.FromResult<Result<string, DomainError>>(new Success<string, DomainError>($"value={v}"));
            },
            errorSelector: _ => DomainError.Invalid);

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}

[TestFixture]
public class TypedResultBindAsync_TaskToSyncBinderTests
{
    [Test]
    public async Task Bind_ToTypedResult_OnSuccess_RunsBinderWithValue()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));

        Result<string> bound = await resultTask.Bind(v => new Success<string>($"value={v}"));

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public async Task Bind_ToTypedResult_OnFailure_ShortCircuitsWithSameMessage()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var binderWasCalled = false;

        Result<string> bound = await resultTask.Bind<int, string>(v =>
        {
            binderWasCalled = true;
            return new Success<string>($"value={v}");
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task Bind_ToResult_OnSuccess_RunsBinder()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));

        Result bound = await resultTask.Bind(() => new Success());

        Assert.That(bound.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public async Task Bind_ToResult_OnFailure_ShortCircuitsWithSameMessage()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var binderWasCalled = false;

        Result bound = await resultTask.Bind(() =>
        {
            binderWasCalled = true;
            return new Success();
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(() => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task Bind_ToTypedErrorResult_OnSuccess_RunsBinderWithValue()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));

        Result<string, DomainError> bound = await resultTask.Bind<int, string, DomainError>(
            v => new Success<string, DomainError>($"value={v}"),
            errorSelector: _ => DomainError.Invalid);

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public async Task Bind_ToTypedErrorResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var binderWasCalled = false;

        Result<string, DomainError> bound = await resultTask.Bind<int, string, DomainError>(
            v =>
            {
                binderWasCalled = true;
                return new Success<string, DomainError>($"value={v}");
            },
            errorSelector: _ => DomainError.Invalid);

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}

[TestFixture]
public class TypedResultBindAsync_TaskToTaskTests
{
    [Test]
    public async Task BindAsync_ToTypedResult_OnSuccess_RunsBinderWithValue()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));

        Result<string> bound = await resultTask.BindAsync(v => Task.FromResult<Result<string>>(new Success<string>($"value={v}")));

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public async Task BindAsync_ToTypedResult_OnFailure_ShortCircuitsWithSameMessage()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var binderWasCalled = false;

        Result<string> bound = await resultTask.BindAsync<int, string>(v =>
        {
            binderWasCalled = true;
            return Task.FromResult<Result<string>>(new Success<string>($"value={v}"));
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task BindAsync_ToResult_OnSuccess_RunsBinder()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));

        Result bound = await resultTask.BindAsync(() => Task.FromResult<Result>(new Success()));

        Assert.That(bound.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public async Task BindAsync_ToResult_OnFailure_ShortCircuitsWithSameMessage()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var binderWasCalled = false;

        Result bound = await resultTask.BindAsync(() =>
        {
            binderWasCalled = true;
            return Task.FromResult<Result>(new Success());
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(() => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task BindAsync_ToTypedErrorResult_OnSuccess_RunsBinderWithValue()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));

        Result<string, DomainError> bound = await resultTask.BindAsync<int, string, DomainError>(
            v => Task.FromResult<Result<string, DomainError>>(new Success<string, DomainError>($"value={v}")),
            errorSelector: _ => DomainError.Invalid);

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public async Task BindAsync_ToTypedErrorResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var binderWasCalled = false;

        Result<string, DomainError> bound = await resultTask.BindAsync<int, string, DomainError>(
            v =>
            {
                binderWasCalled = true;
                return Task.FromResult<Result<string, DomainError>>(new Success<string, DomainError>($"value={v}"));
            },
            errorSelector: _ => DomainError.Invalid);

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}
