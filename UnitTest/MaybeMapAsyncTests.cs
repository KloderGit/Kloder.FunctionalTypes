using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeMapAsync_SyncMaybeToTaskTests
{
    [Test]
    public async Task MapAsync_OnJust_TransformsValue()
    {
        Maybe<int> maybe = Maybe<int>.Just(21);

        Maybe<string> mapped = await maybe.MapAsync(v => Task.FromResult((v * 2).ToString()));

        Assert.That(mapped.Match(v => v, () => "nothing"), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapAsync_OnNothing_SkipsSelectorAndStaysNothing()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();
        var selectorWasCalled = false;

        Maybe<string> mapped = await maybe.MapAsync(v =>
        {
            selectorWasCalled = true;
            return Task.FromResult(v.ToString());
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeMapAsync_TaskToSyncSelectorTests
{
    [Test]
    public async Task Map_OnJust_TransformsValue()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Just(21));

        Maybe<string> mapped = await maybeTask.Map(v => (v * 2).ToString());

        Assert.That(mapped.Match(v => v, () => "nothing"), Is.EqualTo("42"));
    }

    [Test]
    public async Task Map_OnNothing_StaysNothing()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Nothing());

        Maybe<string> mapped = await maybeTask.Map(v => v.ToString());

        Assert.That(mapped.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeMapAsync_TaskToTaskTests
{
    [Test]
    public async Task MapAsync_OnJust_TransformsValue()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Just(21));

        Maybe<string> mapped = await maybeTask.MapAsync(v => Task.FromResult((v * 2).ToString()));

        Assert.That(mapped.Match(v => v, () => "nothing"), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapAsync_OnNothing_StaysNothing()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Nothing());

        Maybe<string> mapped = await maybeTask.MapAsync(v => Task.FromResult(v.ToString()));

        Assert.That(mapped.IsNothing, Is.True);
    }
}
