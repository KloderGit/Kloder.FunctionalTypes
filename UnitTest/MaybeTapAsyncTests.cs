using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeTapAsync_SyncMaybeToTaskTests
{
    [Test]
    public async Task TapAsync_OnJust_RunsActionAndReturnsUnchanged()
    {
        Maybe<int> maybe = Maybe<int>.Just(42);
        var seen = -1;

        Maybe<int> tapped = await maybe.TapAsync(v =>
        {
            seen = v;
            return Task.CompletedTask;
        });

        Assert.That(seen, Is.EqualTo(42));
        Assert.That(tapped, Is.SameAs(maybe));
    }

    [Test]
    public async Task TapAsync_OnNothing_SkipsAction()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();
        var tapWasCalled = false;

        Maybe<int> tapped = await maybe.TapAsync(_ =>
        {
            tapWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(maybe));
    }

    [Test]
    public async Task TapNothingAsync_OnNothing_RunsAction()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();
        var tapNothingWasCalled = false;

        Maybe<int> tapped = await maybe.TapNothingAsync(() =>
        {
            tapNothingWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapNothingWasCalled, Is.True);
        Assert.That(tapped, Is.SameAs(maybe));
    }

    [Test]
    public async Task TapNothingAsync_OnJust_SkipsAction()
    {
        Maybe<int> maybe = Maybe<int>.Just(42);
        var tapNothingWasCalled = false;

        Maybe<int> tapped = await maybe.TapNothingAsync(() =>
        {
            tapNothingWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapNothingWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(maybe));
    }
}

[TestFixture]
public class MaybeTapAsync_TaskToSyncActionTests
{
    [Test]
    public async Task Tap_OnJust_RunsAction()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Just(42));
        var seen = -1;

        Maybe<int> tapped = await maybeTask.Tap(v => seen = v);

        Assert.That(seen, Is.EqualTo(42));
        Assert.That(tapped.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task TapNothing_OnNothing_RunsAction()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Nothing());
        var tapNothingWasCalled = false;

        Maybe<int> tapped = await maybeTask.TapNothing(() => tapNothingWasCalled = true);

        Assert.That(tapNothingWasCalled, Is.True);
        Assert.That(tapped.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeTapAsync_TaskToTaskTests
{
    [Test]
    public async Task TapAsync_OnJust_RunsAction()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Just(42));
        var seen = -1;

        Maybe<int> tapped = await maybeTask.TapAsync(v =>
        {
            seen = v;
            return Task.CompletedTask;
        });

        Assert.That(seen, Is.EqualTo(42));
        Assert.That(tapped.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task TapNothingAsync_OnNothing_RunsAction()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Nothing());
        var tapNothingWasCalled = false;

        Maybe<int> tapped = await maybeTask.TapNothingAsync(() =>
        {
            tapNothingWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapNothingWasCalled, Is.True);
        Assert.That(tapped.IsNothing, Is.True);
    }
}
