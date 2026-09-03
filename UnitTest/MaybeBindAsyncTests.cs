using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeBindAsync_SyncMaybeToTaskTests
{
    [Test]
    public async Task BindAsync_OnSome_RunsBinder()
    {
        Maybe<int> maybe = Maybe<int>.Some(21);

        Maybe<string> bound = await maybe.BindAsync(v => Task.FromResult(Maybe<string>.Some((v * 2).ToString())));

        Assert.That(bound.Match(v => v, () => "none"), Is.EqualTo("42"));
    }

    [Test]
    public async Task BindAsync_OnNone_SkipsBinderAndStaysNone()
    {
        Maybe<int> maybe = Maybe<int>.None();
        var binderWasCalled = false;

        Maybe<string> bound = await maybe.BindAsync(v =>
        {
            binderWasCalled = true;
            return Task.FromResult(Maybe<string>.Some(v.ToString()));
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeBindAsync_TaskToSyncBinderTests
{
    [Test]
    public async Task Bind_OnSome_RunsBinder()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Some(21));

        Maybe<string> bound = await maybeTask.Bind(v => Maybe<string>.Some((v * 2).ToString()));

        Assert.That(bound.Match(v => v, () => "none"), Is.EqualTo("42"));
    }

    [Test]
    public async Task Bind_OnNone_StaysNone()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.None());

        Maybe<string> bound = await maybeTask.Bind(v => Maybe<string>.Some(v.ToString()));

        Assert.That(bound.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeBindAsync_TaskToTaskTests
{
    [Test]
    public async Task BindAsync_OnSome_RunsBinder()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Some(21));

        Maybe<string> bound = await maybeTask.BindAsync(v => Task.FromResult(Maybe<string>.Some((v * 2).ToString())));

        Assert.That(bound.Match(v => v, () => "none"), Is.EqualTo("42"));
    }

    [Test]
    public async Task BindAsync_OnNone_StaysNone()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.None());

        Maybe<string> bound = await maybeTask.BindAsync(v => Task.FromResult(Maybe<string>.Some(v.ToString())));

        Assert.That(bound.IsNone, Is.True);
    }
}
