using FunctionalTypes.TypedErrorResult;

namespace UnitTest;

[TestFixture]
public class TypedErrorResultTraverseTests
{
    [Test]
    public void Traverse_AllSucceed_ProducesSuccessWithAllValues()
    {
        var source = new[] { 1, 2, 3 };

        Result<IEnumerable<int>, DomainError> result = source.Traverse(v => new Success<int, DomainError>(v * 2));

        Assert.That(result.Match(v => v, _ => []), Is.EqualTo(new[] { 2, 4, 6 }));
    }

    [Test]
    public void Traverse_OneFails_ShortCircuitsWithFirstFailureAndSkipsRemainingItems()
    {
        var source = new[] { 1, 2, 3, 4 };
        var processed = new List<int>();

        Result<IEnumerable<int>, DomainError> result = source.Traverse(v =>
        {
            processed.Add(v);
            return v == 2
                ? new Failure<int, DomainError>(DomainError.Invalid)
                : (Result<int, DomainError>)new Success<int, DomainError>(v);
        });

        Assert.That(result.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
        Assert.That(processed, Is.EqualTo(new[] { 1, 2 }));
    }

    [Test]
    public void Sequence_AllSucceed_ProducesSuccessWithAllValues()
    {
        var source = new[]
            {
                new Success<int, DomainError>(1), new Success<int, DomainError>(2), new Success<int, DomainError>(3)
            }
            .Cast<Result<int, DomainError>>();

        Result<IEnumerable<int>, DomainError> result = source.Sequence();

        Assert.That(result.Match(v => v, _ => []), Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public void Sequence_OneFails_ProducesFirstFailure()
    {
        Result<int, DomainError>[] source =
        [
            new Success<int, DomainError>(1),
            new Failure<int, DomainError>(DomainError.NotFound),
            new Success<int, DomainError>(3)
        ];

        Result<IEnumerable<int>, DomainError> result = source.Sequence();

        Assert.That(result.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}
