using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeTraverseTests
{
    [Test]
    public void Traverse_AllSome_ProducesSomeWithAllValues()
    {
        var source = new[] { 1, 2, 3 };

        Maybe<IEnumerable<int>> result = source.Traverse(v => Maybe<int>.Some(v * 2));

        Assert.That(result.Match(v => v, () => []), Is.EqualTo(new[] { 2, 4, 6 }));
    }

    [Test]
    public void Traverse_OneNone_ShortCircuitsAndSkipsRemainingItems()
    {
        var source = new[] { 1, 2, 3, 4 };
        var processed = new List<int>();

        Maybe<IEnumerable<int>> result = source.Traverse(v =>
        {
            processed.Add(v);
            return v == 2 ? Maybe<int>.None() : Maybe<int>.Some(v);
        });

        Assert.That(result.IsNone, Is.True);
        Assert.That(processed, Is.EqualTo(new[] { 1, 2 }));
    }

    [Test]
    public void Sequence_AllSome_ProducesSomeWithAllValues()
    {
        var source = new[] { Maybe<int>.Some(1), Maybe<int>.Some(2), Maybe<int>.Some(3) };

        Maybe<IEnumerable<int>> result = source.Sequence();

        Assert.That(result.Match(v => v, () => []), Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public void Sequence_OneNone_ProducesNone()
    {
        var source = new[] { Maybe<int>.Some(1), Maybe<int>.None(), Maybe<int>.Some(3) };

        Maybe<IEnumerable<int>> result = source.Sequence();

        Assert.That(result.IsNone, Is.True);
    }
}
