using FunctionalTypes.TypedErrorResult;

namespace UnitTest;

[TestFixture]
public class TypedErrorResultTapAsync_SyncResultToTaskTests
{
    [Test]
    public async Task TapAsync_OnSuccess_RunsActionWithValueAndReturnsUnchangedResult()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);
        var seen = -1;

        Result<int, DomainError> tapped = await result.TapAsync(v =>
        {
            seen = v;
            return Task.CompletedTask;
        });

        Assert.That(seen, Is.EqualTo(42));
        Assert.That(tapped, Is.SameAs(result));
    }

    [Test]
    public async Task TapAsync_OnFailure_SkipsAction()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);
        var tapWasCalled = false;

        Result<int, DomainError> tapped = await result.TapAsync(_ =>
        {
            tapWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(result));
    }

    [Test]
    public async Task TapAsync_IgnoringValue_OnSuccess_RunsAction()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);
        var tapWasCalled = false;

        Result<int, DomainError> tapped = await result.TapAsync(() =>
        {
            tapWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapWasCalled, Is.True);
        Assert.That(tapped, Is.SameAs(result));
    }

    [Test]
    public async Task TapAsync_IgnoringValue_OnFailure_SkipsAction()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);
        var tapWasCalled = false;

        Result<int, DomainError> tapped = await result.TapAsync(() =>
        {
            tapWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(result));
    }

    [Test]
    public async Task TapErrorAsync_OnFailure_RunsActionWithError()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);
        var seen = DomainError.Invalid;

        Result<int, DomainError> tapped = await result.TapErrorAsync(e =>
        {
            seen = e;
            return Task.CompletedTask;
        });

        Assert.That(seen, Is.EqualTo(DomainError.NotFound));
        Assert.That(tapped, Is.SameAs(result));
    }

    [Test]
    public async Task TapErrorAsync_OnSuccess_SkipsAction()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);
        var tapErrorWasCalled = false;

        Result<int, DomainError> tapped = await result.TapErrorAsync(_ =>
        {
            tapErrorWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapErrorWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(result));
    }

    [Test]
    public async Task TapErrorAsync_IgnoringError_OnFailure_RunsAction()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);
        var tapErrorWasCalled = false;

        Result<int, DomainError> tapped = await result.TapErrorAsync(() =>
        {
            tapErrorWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapErrorWasCalled, Is.True);
        Assert.That(tapped, Is.SameAs(result));
    }

    [Test]
    public async Task TapErrorAsync_IgnoringError_OnSuccess_SkipsAction()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);
        var tapErrorWasCalled = false;

        Result<int, DomainError> tapped = await result.TapErrorAsync(() =>
        {
            tapErrorWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapErrorWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(result));
    }
}

[TestFixture]
public class TypedErrorResultTapAsync_TaskToSyncActionTests
{
    [Test]
    public async Task Tap_OnSuccess_RunsActionWithValue()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42));
        var seen = -1;

        Result<int, DomainError> tapped = await resultTask.Tap(v => seen = v);

        Assert.That(seen, Is.EqualTo(42));
        Assert.That(tapped.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task TapError_OnFailure_RunsActionWithError()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));
        var seen = DomainError.Invalid;

        Result<int, DomainError> tapped = await resultTask.TapError(e => seen = e);

        Assert.That(seen, Is.EqualTo(DomainError.NotFound));
        Assert.That(tapped.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}

[TestFixture]
public class TypedErrorResultTapAsync_TaskToTaskTests
{
    [Test]
    public async Task TapAsync_OnSuccess_RunsActionWithValue()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(42));
        var seen = -1;

        Result<int, DomainError> tapped = await resultTask.TapAsync(v =>
        {
            seen = v;
            return Task.CompletedTask;
        });

        Assert.That(seen, Is.EqualTo(42));
        Assert.That(tapped.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task TapErrorAsync_OnFailure_RunsActionWithError()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));
        var seen = DomainError.Invalid;

        Result<int, DomainError> tapped = await resultTask.TapErrorAsync(e =>
        {
            seen = e;
            return Task.CompletedTask;
        });

        Assert.That(seen, Is.EqualTo(DomainError.NotFound));
        Assert.That(tapped.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}
