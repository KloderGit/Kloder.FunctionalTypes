using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeMapAsync_SyncMaybeToTaskTests
{
    [Test]
    public async Task MapAsync_OnSome_TransformsValue()
    {
        Maybe<int> maybe = Maybe<int>.Some(21);

        Maybe<string> mapped = await maybe.MapAsync(v => Task.FromResult((v * 2).ToString()));

        Assert.That(mapped.Match(v => v, () => "none"), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapAsync_OnNone_SkipsSelectorAndStaysNone()
    {
        Maybe<int> maybe = Maybe<int>.None();
        var selectorWasCalled = false;

        Maybe<string> mapped = await maybe.MapAsync(v =>
        {
            selectorWasCalled = true;
            return Task.FromResult(v.ToString());
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeMapAsync_TaskToSyncSelectorTests
{
    [Test]
    public async Task Map_OnSome_TransformsValue()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Some(21));

        Maybe<string> mapped = await maybeTask.Map(v => (v * 2).ToString());

        Assert.That(mapped.Match(v => v, () => "none"), Is.EqualTo("42"));
    }

    [Test]
    public async Task Map_OnNone_StaysNone()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.None());

        Maybe<string> mapped = await maybeTask.Map(v => v.ToString());

        Assert.That(mapped.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeMapAsync_TaskToTaskTests
{
    [Test]
    public async Task MapAsync_OnSome_TransformsValue()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Some(21));

        Maybe<string> mapped = await maybeTask.MapAsync(v => Task.FromResult((v * 2).ToString()));

        Assert.That(mapped.Match(v => v, () => "none"), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapAsync_OnNone_StaysNone()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.None());

        Maybe<string> mapped = await maybeTask.MapAsync(v => Task.FromResult(v.ToString()));

        Assert.That(mapped.IsNone, Is.True);
    }
}
