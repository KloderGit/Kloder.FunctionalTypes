using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeTraverseAsyncTests
{
    [Test]
    public async Task TraverseAsync_AllSome_ProducesSomeWithAllValues()
    {
        var source = new[] { 1, 2, 3 };

        Maybe<IEnumerable<int>> result = await source.TraverseAsync(v => Task.FromResult(Maybe<int>.Some(v * 2)));

        Assert.That(result.Match(v => v, () => []), Is.EqualTo(new[] { 2, 4, 6 }));
    }

    [Test]
    public async Task TraverseAsync_OneNone_ShortCircuitsWithoutAwaitingRemainingItems()
    {
        var source = new[] { 1, 2, 3, 4 };
        var processed = new List<int>();

        Maybe<IEnumerable<int>> result = await source.TraverseAsync(async v =>
        {
            processed.Add(v);
            await Task.Yield();
            return v == 2 ? Maybe<int>.None() : Maybe<int>.Some(v);
        });

        Assert.That(result.IsNone, Is.True);
        Assert.That(processed, Is.EqualTo(new[] { 1, 2 }));
    }

    [Test]
    public async Task SequenceAsync_AllSome_ProducesSomeWithAllValues()
    {
        var source = new[]
        {
            Task.FromResult(Maybe<int>.Some(1)),
            Task.FromResult(Maybe<int>.Some(2)),
            Task.FromResult(Maybe<int>.Some(3))
        };

        Maybe<IEnumerable<int>> result = await source.SequenceAsync();

        Assert.That(result.Match(v => v, () => []), Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public async Task SequenceAsync_OneNone_ProducesNone()
    {
        var source = new[]
        {
            Task.FromResult(Maybe<int>.Some(1)),
            Task.FromResult(Maybe<int>.None()),
            Task.FromResult(Maybe<int>.Some(3))
        };

        Maybe<IEnumerable<int>> result = await source.SequenceAsync();

        Assert.That(result.IsNone, Is.True);
    }
}
