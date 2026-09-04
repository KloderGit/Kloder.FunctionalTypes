using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeTraverseTests
{
    [Test]
    public void Traverse_AllJust_ProducesJustWithAllValues()
    {
        var source = new[] { 1, 2, 3 };

        Maybe<IEnumerable<int>> result = source.Traverse(v => Maybe<int>.Just(v * 2));

        Assert.That(result.Match(v => v, () => []), Is.EqualTo(new[] { 2, 4, 6 }));
    }

    [Test]
    public void Traverse_OneNothing_ShortCircuitsAndSkipsRemainingItems()
    {
        var source = new[] { 1, 2, 3, 4 };
        var processed = new List<int>();

        Maybe<IEnumerable<int>> result = source.Traverse(v =>
        {
            processed.Add(v);
            return v == 2 ? Maybe<int>.Nothing() : Maybe<int>.Just(v);
        });

        Assert.That(result.IsNothing, Is.True);
        Assert.That(processed, Is.EqualTo(new[] { 1, 2 }));
    }

    [Test]
    public void Sequence_AllJust_ProducesJustWithAllValues()
    {
        var source = new[] { Maybe<int>.Just(1), Maybe<int>.Just(2), Maybe<int>.Just(3) };

        Maybe<IEnumerable<int>> result = source.Sequence();

        Assert.That(result.Match(v => v, () => []), Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public void Sequence_OneNothing_ProducesNothing()
    {
        var source = new[] { Maybe<int>.Just(1), Maybe<int>.Nothing(), Maybe<int>.Just(3) };

        Maybe<IEnumerable<int>> result = source.Sequence();

        Assert.That(result.IsNothing, Is.True);
    }
}
