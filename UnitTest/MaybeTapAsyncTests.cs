using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeTapAsync_SyncMaybeToTaskTests
{
    [Test]
    public async Task TapAsync_OnSome_RunsActionAndReturnsUnchanged()
    {
        Maybe<int> maybe = Maybe<int>.Some(42);
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
    public async Task TapAsync_OnNone_SkipsAction()
    {
        Maybe<int> maybe = Maybe<int>.None();
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
    public async Task TapNoneAsync_OnNone_RunsAction()
    {
        Maybe<int> maybe = Maybe<int>.None();
        var tapNoneWasCalled = false;

        Maybe<int> tapped = await maybe.TapNoneAsync(() =>
        {
            tapNoneWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapNoneWasCalled, Is.True);
        Assert.That(tapped, Is.SameAs(maybe));
    }

    [Test]
    public async Task TapNoneAsync_OnSome_SkipsAction()
    {
        Maybe<int> maybe = Maybe<int>.Some(42);
        var tapNoneWasCalled = false;

        Maybe<int> tapped = await maybe.TapNoneAsync(() =>
        {
            tapNoneWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapNoneWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(maybe));
    }
}

[TestFixture]
public class MaybeTapAsync_TaskToSyncActionTests
{
    [Test]
    public async Task Tap_OnSome_RunsAction()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Some(42));
        var seen = -1;

        Maybe<int> tapped = await maybeTask.Tap(v => seen = v);

        Assert.That(seen, Is.EqualTo(42));
        Assert.That(tapped.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task TapNone_OnNone_RunsAction()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.None());
        var tapNoneWasCalled = false;

        Maybe<int> tapped = await maybeTask.TapNone(() => tapNoneWasCalled = true);

        Assert.That(tapNoneWasCalled, Is.True);
        Assert.That(tapped.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeTapAsync_TaskToTaskTests
{
    [Test]
    public async Task TapAsync_OnSome_RunsAction()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Some(42));
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
    public async Task TapNoneAsync_OnNone_RunsAction()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.None());
        var tapNoneWasCalled = false;

        Maybe<int> tapped = await maybeTask.TapNoneAsync(() =>
        {
            tapNoneWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapNoneWasCalled, Is.True);
        Assert.That(tapped.IsNone, Is.True);
    }
}
