using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeBindAsync_SyncMaybeToTaskTests
{
    [Test]
    public async Task BindAsync_OnJust_RunsBinder()
    {
        Maybe<int> maybe = Maybe<int>.Just(21);

        Maybe<string> bound = await maybe.BindAsync(v => Task.FromResult(Maybe<string>.Just((v * 2).ToString())));

        Assert.That(bound.Match(v => v, () => "nothing"), Is.EqualTo("42"));
    }

    [Test]
    public async Task BindAsync_OnNothing_SkipsBinderAndStaysNothing()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();
        var binderWasCalled = false;

        Maybe<string> bound = await maybe.BindAsync(v =>
        {
            binderWasCalled = true;
            return Task.FromResult(Maybe<string>.Just(v.ToString()));
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeBindAsync_TaskToSyncBinderTests
{
    [Test]
    public async Task Bind_OnJust_RunsBinder()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Just(21));

        Maybe<string> bound = await maybeTask.Bind(v => Maybe<string>.Just((v * 2).ToString()));

        Assert.That(bound.Match(v => v, () => "nothing"), Is.EqualTo("42"));
    }

    [Test]
    public async Task Bind_OnNothing_StaysNothing()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Nothing());

        Maybe<string> bound = await maybeTask.Bind(v => Maybe<string>.Just(v.ToString()));

        Assert.That(bound.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeBindAsync_TaskToTaskTests
{
    [Test]
    public async Task BindAsync_OnJust_RunsBinder()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Just(21));

        Maybe<string> bound = await maybeTask.BindAsync(v => Task.FromResult(Maybe<string>.Just((v * 2).ToString())));

        Assert.That(bound.Match(v => v, () => "nothing"), Is.EqualTo("42"));
    }

    [Test]
    public async Task BindAsync_OnNothing_StaysNothing()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Nothing());

        Maybe<string> bound = await maybeTask.BindAsync(v => Task.FromResult(Maybe<string>.Just(v.ToString())));

        Assert.That(bound.IsNothing, Is.True);
    }
}
