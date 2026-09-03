using FunctionalTypes.SimpleResult;
using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class TypedResultTapAsync_SyncResultToTaskTests
{
    [Test]
    public async Task TapAsync_OnSuccess_RunsActionWithValueAndReturnsUnchangedResult()
    {
        Result<int> result = new Success<int>(42);
        var seen = -1;

        Result<int> tapped = await result.TapAsync(v =>
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
        Result<int> result = new Failure<int>("boom");
        var tapWasCalled = false;

        Result<int> tapped = await result.TapAsync(_ =>
        {
            tapWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(result));
    }

    [Test]
    public async Task TapErrorAsync_OnFailure_RunsActionWithMessage()
    {
        Result<int> result = new Failure<int>("boom");
        var seen = "";

        Result<int> tapped = await result.TapErrorAsync(msg =>
        {
            seen = msg;
            return Task.CompletedTask;
        });

        Assert.That(seen, Is.EqualTo("boom"));
        Assert.That(tapped, Is.SameAs(result));
    }

    [Test]
    public async Task TapErrorAsync_OnSuccess_SkipsAction()
    {
        Result<int> result = new Success<int>(42);
        var tapErrorWasCalled = false;

        Result<int> tapped = await result.TapErrorAsync(_ =>
        {
            tapErrorWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapErrorWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(result));
    }

    [Test]
    public async Task TapAsync_ToSimpleResult_OnSuccess_RunsActionAndBridges()
    {
        Result<int> result = new Success<int>(42);
        var tapWasCalled = false;

        Result tapped = await result.TapAsync(() =>
        {
            tapWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapWasCalled, Is.True);
        Assert.That(tapped.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public async Task TapAsync_ToSimpleResult_OnFailure_SkipsActionAndBridges()
    {
        Result<int> result = new Failure<int>("boom");
        var tapWasCalled = false;

        Result tapped = await result.TapAsync(() =>
        {
            tapWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapWasCalled, Is.False);
        Assert.That(tapped.Match(() => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task TapErrorAsync_ToSimpleResult_OnFailure_RunsActionAndBridges()
    {
        Result<int> result = new Failure<int>("boom");
        var tapErrorWasCalled = false;

        Result tapped = await result.TapErrorAsync(() =>
        {
            tapErrorWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapErrorWasCalled, Is.True);
        Assert.That(tapped.Match(() => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task TapErrorAsync_ToSimpleResult_OnSuccess_SkipsActionAndBridges()
    {
        Result<int> result = new Success<int>(42);
        var tapErrorWasCalled = false;

        Result tapped = await result.TapErrorAsync(() =>
        {
            tapErrorWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapErrorWasCalled, Is.False);
        Assert.That(tapped.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }
}

[TestFixture]
public class TypedResultTapAsync_TaskToSyncActionTests
{
    [Test]
    public async Task Tap_OnSuccess_RunsActionWithValue()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));
        var seen = -1;

        Result<int> tapped = await resultTask.Tap(v => seen = v);

        Assert.That(seen, Is.EqualTo(42));
        Assert.That(tapped.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task TapError_OnFailure_RunsActionWithMessage()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var seen = "";

        Result<int> tapped = await resultTask.TapError(msg => seen = msg);

        Assert.That(seen, Is.EqualTo("boom"));
        Assert.That(tapped.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task Tap_ToSimpleResult_OnSuccess_RunsAction()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));
        var tapWasCalled = false;

        Result tapped = await resultTask.Tap(() => tapWasCalled = true);

        Assert.That(tapWasCalled, Is.True);
        Assert.That(tapped.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public async Task TapError_ToSimpleResult_OnFailure_RunsAction()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var tapErrorWasCalled = false;

        Result tapped = await resultTask.TapError(() => tapErrorWasCalled = true);

        Assert.That(tapErrorWasCalled, Is.True);
        Assert.That(tapped.Match(() => "success", e => e), Is.EqualTo("boom"));
    }
}

[TestFixture]
public class TypedResultTapAsync_TaskToTaskTests
{
    [Test]
    public async Task TapAsync_OnSuccess_RunsActionWithValue()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));
        var seen = -1;

        Result<int> tapped = await resultTask.TapAsync(v =>
        {
            seen = v;
            return Task.CompletedTask;
        });

        Assert.That(seen, Is.EqualTo(42));
        Assert.That(tapped.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task TapErrorAsync_OnFailure_RunsActionWithMessage()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var seen = "";

        Result<int> tapped = await resultTask.TapErrorAsync(msg =>
        {
            seen = msg;
            return Task.CompletedTask;
        });

        Assert.That(seen, Is.EqualTo("boom"));
        Assert.That(tapped.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task TapAsync_ToSimpleResult_OnSuccess_RunsAction()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));
        var tapWasCalled = false;

        Result tapped = await resultTask.TapAsync(() =>
        {
            tapWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapWasCalled, Is.True);
        Assert.That(tapped.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public async Task TapErrorAsync_ToSimpleResult_OnFailure_RunsAction()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Failure<int>("boom"));
        var tapErrorWasCalled = false;

        Result tapped = await resultTask.TapErrorAsync(() =>
        {
            tapErrorWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapErrorWasCalled, Is.True);
        Assert.That(tapped.Match(() => "success", e => e), Is.EqualTo("boom"));
    }
}
