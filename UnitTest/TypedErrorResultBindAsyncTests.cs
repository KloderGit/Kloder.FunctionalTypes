using FunctionalTypes.TypedErrorResult;

namespace UnitTest;

[TestFixture]
public class TypedErrorResultBindAsync_SyncResultToTaskTests
{
    [Test]
    public async Task BindAsync_OnSuccess_RunsBinderWithValue()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);

        Result<string, DomainError> bound = await result.BindAsync(
            v => Task.FromResult<Result<string, DomainError>>(new Success<string, DomainError>($"value={v}")));

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public async Task BindAsync_OnFailure_ShortCircuitsWithSameError()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);
        var binderWasCalled = false;

        Result<string, DomainError> bound = await result.BindAsync<int, string, DomainError>(v =>
        {
            binderWasCalled = true;
            return Task.FromResult<Result<string, DomainError>>(new Success<string, DomainError>($"value={v}"));
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}

[TestFixture]
public class TypedErrorResultBindAsync_TaskToSyncBinderTests
{
    [Test]
    public async Task Bind_OnSuccess_RunsBinderWithValue()
    {
        Task<Result<int, DomainError>> resultTask = Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42));

        Result<string, DomainError> bound = await resultTask.Bind(v => new Success<string, DomainError>($"value={v}"));

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public async Task Bind_OnFailure_ShortCircuitsWithSameError()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.Invalid));
        var binderWasCalled = false;

        Result<string, DomainError> bound = await resultTask.Bind<int, string, DomainError>(v =>
        {
            binderWasCalled = true;
            return new Success<string, DomainError>($"value={v}");
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}

[TestFixture]
public class TypedErrorResultBindAsync_TaskToTaskTests
{
    [Test]
    public async Task BindAsync_OnSuccess_RunsBinderWithValue()
    {
        Task<Result<int, DomainError>> resultTask = Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42));

        Result<string, DomainError> bound = await resultTask.BindAsync(
            v => Task.FromResult<Result<string, DomainError>>(new Success<string, DomainError>($"value={v}")));

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public async Task BindAsync_OnFailure_ShortCircuitsWithSameError()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));
        var binderWasCalled = false;

        Result<string, DomainError> bound = await resultTask.BindAsync<int, string, DomainError>(v =>
        {
            binderWasCalled = true;
            return Task.FromResult<Result<string, DomainError>>(new Success<string, DomainError>($"value={v}"));
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}
