using FunctionalTypes.SimpleResult;
using FunctionalTypes.TypedErrorResult;
using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class SimpleResultBindAsync_SyncResultToTaskTests
{
    [Test]
    public async Task BindAsync_ToResult_OnSuccess_RunsBinderAndReturnsItsResult()
    {
        Result result = new Success();

        Result bound = await result.BindAsync(() => Task.FromResult<Result>(new Success()));

        Assert.That(bound.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public async Task BindAsync_ToResult_OnFailure_ShortCircuitsWithoutRunningBinder()
    {
        Result result = new Failure("boom");
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
    public async Task BindAsync_ToTypedResult_OnSuccess_RunsBinderAndReturnsItsResult()
    {
        Result result = new Success();

        Result<int> bound = await result.BindAsync(() => Task.FromResult<Result<int>>(new Success<int>(42)));

        Assert.That(bound.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task BindAsync_ToTypedResult_OnFailure_ShortCircuitsWithSameMessage()
    {
        Result result = new Failure("boom");
        var binderWasCalled = false;

        Result<int> bound = await result.BindAsync<int>(() =>
        {
            binderWasCalled = true;
            return Task.FromResult<Result<int>>(new Success<int>(42));
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task BindAsync_ToTypedErrorResult_OnSuccess_RunsBinderAndReturnsItsResult()
    {
        Result result = new Success();

        Result<int, DomainError> bound = await result.BindAsync<int, DomainError>(
            () => Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42)),
            errorSelector: _ => DomainError.Invalid);

        Assert.That(bound.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task BindAsync_ToTypedErrorResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Result result = new Failure("not-found");
        var binderWasCalled = false;

        Result<int, DomainError> bound = await result.BindAsync<int, DomainError>(
            () =>
            {
                binderWasCalled = true;
                return Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42));
            },
            errorSelector: msg => msg == "not-found" ? DomainError.NotFound : DomainError.Invalid);

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}

[TestFixture]
public class SimpleResultBindAsync_TaskToSyncBinderTests
{
    [Test]
    public async Task Bind_ToResult_OnSuccess_RunsBinder()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        Result bound = await resultTask.Bind(() => new Success());

        Assert.That(bound.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public async Task Bind_ToResult_OnFailure_ShortCircuitsWithoutRunningBinder()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
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
    public async Task Bind_ToTypedResult_OnSuccess_RunsBinder()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        Result<int> bound = await resultTask.Bind(() => new Success<int>(42));

        Assert.That(bound.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task Bind_ToTypedResult_OnFailure_ShortCircuitsWithSameMessage()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var binderWasCalled = false;

        Result<int> bound = await resultTask.Bind<int>(() =>
        {
            binderWasCalled = true;
            return new Success<int>(42);
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task Bind_ToTypedErrorResult_OnSuccess_RunsBinder()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        Result<int, DomainError> bound = await resultTask.Bind(
            () => new Success<int, DomainError>(42),
            errorSelector: _ => DomainError.Invalid);

        Assert.That(bound.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task Bind_ToTypedErrorResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var binderWasCalled = false;

        Result<int, DomainError> bound = await resultTask.Bind<int, DomainError>(
            () =>
            {
                binderWasCalled = true;
                return new Success<int, DomainError>(42);
            },
            errorSelector: _ => DomainError.Invalid);

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}

[TestFixture]
public class SimpleResultBindAsync_TaskToTaskTests
{
    [Test]
    public async Task BindAsync_ToResult_OnSuccess_RunsBinder()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        Result bound = await resultTask.BindAsync(() => Task.FromResult<Result>(new Success()));

        Assert.That(bound.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public async Task BindAsync_ToResult_OnFailure_ShortCircuitsWithoutRunningBinder()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
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
    public async Task BindAsync_ToTypedResult_OnSuccess_RunsBinder()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        Result<int> bound = await resultTask.BindAsync(() => Task.FromResult<Result<int>>(new Success<int>(42)));

        Assert.That(bound.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task BindAsync_ToTypedResult_OnFailure_ShortCircuitsWithSameMessage()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var binderWasCalled = false;

        Result<int> bound = await resultTask.BindAsync<int>(() =>
        {
            binderWasCalled = true;
            return Task.FromResult<Result<int>>(new Success<int>(42));
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task BindAsync_ToTypedErrorResult_OnSuccess_RunsBinder()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());

        Result<int, DomainError> bound = await resultTask.BindAsync<int, DomainError>(
            () => Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42)),
            errorSelector: _ => DomainError.Invalid);

        Assert.That(bound.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task BindAsync_ToTypedErrorResult_OnFailure_ShortCircuitsAndConvertsError()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var binderWasCalled = false;

        Result<int, DomainError> bound = await resultTask.BindAsync<int, DomainError>(
            () =>
            {
                binderWasCalled = true;
                return Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42));
            },
            errorSelector: _ => DomainError.Invalid);

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}
