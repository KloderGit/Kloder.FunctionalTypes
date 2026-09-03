using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class TypedResultTraverseTests
{
    [Test]
    public void Traverse_AllSucceed_ProducesSuccessWithAllValues()
    {
        var source = new[] { 1, 2, 3 };

        Result<IEnumerable<int>> result = source.Traverse(v => new Success<int>(v * 2));

        Assert.That(result.Match(v => v, _ => []), Is.EqualTo(new[] { 2, 4, 6 }));
    }

    [Test]
    public void Traverse_OneFails_ShortCircuitsWithFirstFailureAndSkipsRemainingItems()
    {
        var source = new[] { 1, 2, 3, 4 };
        var processed = new List<int>();

        Result<IEnumerable<int>> result = source.Traverse(v =>
        {
            processed.Add(v);
            return v == 2 ? new Failure<int>("bad item") : (Result<int>)new Success<int>(v);
        });

        Assert.That(result.Match(_ => "success", e => e), Is.EqualTo("bad item"));
        Assert.That(processed, Is.EqualTo(new[] { 1, 2 }));
    }

    [Test]
    public void Traverse_EmptySource_ProducesSuccessWithEmptyCollection()
    {
        var source = Array.Empty<int>();

        Result<IEnumerable<int>> result = source.Traverse(v => new Success<int>(v));

        Assert.That(result.Match(v => v, _ => []), Is.Empty);
    }

    [Test]
    public void Sequence_AllSucceed_ProducesSuccessWithAllValues()
    {
        Result<int>[] source = [new Success<int>(1), new Success<int>(2), new Success<int>(3)];

        Result<IEnumerable<int>> result = source.Sequence();

        Assert.That(result.Match(v => v, _ => []), Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public void Sequence_OneFails_ProducesFirstFailure()
    {
        Result<int>[] source = [new Success<int>(1), new Failure<int>("boom"), new Success<int>(3)];

        Result<IEnumerable<int>> result = source.Sequence();

        Assert.That(result.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }
}
