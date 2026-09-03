using FunctionalTypes.SimpleResult;

namespace UnitTest;

[TestFixture]
public class SimpleResultTapAsync_SyncResultToTaskTests
{
    [Test]
    public async Task TapAsync_OnSuccess_RunsActionAndReturnsUnchangedResult()
    {
        Result result = new Success();
        var tapWasCalled = false;

        Result tapped = await result.TapAsync(() =>
        {
            tapWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapWasCalled, Is.True);
        Assert.That(tapped, Is.SameAs(result));
    }

    [Test]
    public async Task TapAsync_OnFailure_SkipsAction()
    {
        Result result = new Failure("boom");
        var tapWasCalled = false;

        Result tapped = await result.TapAsync(() =>
        {
            tapWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(result));
    }

    [Test]
    public async Task TapErrorAsync_OnFailure_RunsAction()
    {
        Result result = new Failure("boom");
        var tapErrorWasCalled = false;

        Result tapped = await result.TapErrorAsync(() =>
        {
            tapErrorWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapErrorWasCalled, Is.True);
        Assert.That(tapped, Is.SameAs(result));
    }

    [Test]
    public async Task TapErrorAsync_OnSuccess_SkipsAction()
    {
        Result result = new Success();
        var tapErrorWasCalled = false;

        Result tapped = await result.TapErrorAsync(() =>
        {
            tapErrorWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapErrorWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(result));
    }
}

[TestFixture]
public class SimpleResultTapAsync_TaskToSyncActionTests
{
    [Test]
    public async Task Tap_OnSuccess_RunsAction()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());
        var tapWasCalled = false;

        Result tapped = await resultTask.Tap(() => tapWasCalled = true);

        Assert.That(tapWasCalled, Is.True);
        Assert.That(tapped.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public async Task Tap_OnFailure_SkipsAction()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var tapWasCalled = false;

        Result tapped = await resultTask.Tap(() => tapWasCalled = true);

        Assert.That(tapWasCalled, Is.False);
        Assert.That(tapped.Match(() => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task TapError_OnFailure_RunsAction()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var tapErrorWasCalled = false;

        Result tapped = await resultTask.TapError(() => tapErrorWasCalled = true);

        Assert.That(tapErrorWasCalled, Is.True);
        Assert.That(tapped.Match(() => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task TapError_OnSuccess_SkipsAction()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());
        var tapErrorWasCalled = false;

        Result tapped = await resultTask.TapError(() => tapErrorWasCalled = true);

        Assert.That(tapErrorWasCalled, Is.False);
        Assert.That(tapped.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }
}

[TestFixture]
public class SimpleResultTapAsync_TaskToTaskTests
{
    [Test]
    public async Task TapAsync_OnSuccess_RunsAction()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());
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
    public async Task TapAsync_OnFailure_SkipsAction()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var tapWasCalled = false;

        Result tapped = await resultTask.TapAsync(() =>
        {
            tapWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapWasCalled, Is.False);
        Assert.That(tapped.Match(() => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task TapErrorAsync_OnFailure_RunsAction()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Failure("boom"));
        var tapErrorWasCalled = false;

        Result tapped = await resultTask.TapErrorAsync(() =>
        {
            tapErrorWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapErrorWasCalled, Is.True);
        Assert.That(tapped.Match(() => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task TapErrorAsync_OnSuccess_SkipsAction()
    {
        Task<Result> resultTask = Task.FromResult<Result>(new Success());
        var tapErrorWasCalled = false;

        Result tapped = await resultTask.TapErrorAsync(() =>
        {
            tapErrorWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapErrorWasCalled, Is.False);
        Assert.That(tapped.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }
}
