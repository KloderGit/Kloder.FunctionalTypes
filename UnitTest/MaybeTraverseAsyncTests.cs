using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeTraverseAsyncTests
{
    [Test]
    public async Task TraverseAsync_AllJust_ProducesJustWithAllValues()
    {
        var source = new[] { 1, 2, 3 };

        Maybe<IEnumerable<int>> result = await source.TraverseAsync(v => Task.FromResult(Maybe<int>.Just(v * 2)));

        Assert.That(result.Match(v => v, () => []), Is.EqualTo(new[] { 2, 4, 6 }));
    }

    [Test]
    public async Task TraverseAsync_OneNothing_ShortCircuitsWithoutAwaitingRemainingItems()
    {
        var source = new[] { 1, 2, 3, 4 };
        var processed = new List<int>();

        Maybe<IEnumerable<int>> result = await source.TraverseAsync(async v =>
        {
            processed.Add(v);
            await Task.Yield();
            return v == 2 ? Maybe<int>.Nothing() : Maybe<int>.Just(v);
        });

        Assert.That(result.IsNothing, Is.True);
        Assert.That(processed, Is.EqualTo(new[] { 1, 2 }));
    }

    [Test]
    public async Task SequenceAsync_AllJust_ProducesJustWithAllValues()
    {
        var source = new[]
        {
            Task.FromResult(Maybe<int>.Just(1)),
            Task.FromResult(Maybe<int>.Just(2)),
            Task.FromResult(Maybe<int>.Just(3))
        };

        Maybe<IEnumerable<int>> result = await source.SequenceAsync();

        Assert.That(result.Match(v => v, () => []), Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public async Task SequenceAsync_OneNothing_ProducesNothing()
    {
        var source = new[]
        {
            Task.FromResult(Maybe<int>.Just(1)),
            Task.FromResult(Maybe<int>.Nothing()),
            Task.FromResult(Maybe<int>.Just(3))
        };

        Maybe<IEnumerable<int>> result = await source.SequenceAsync();

        Assert.That(result.IsNothing, Is.True);
    }
}
