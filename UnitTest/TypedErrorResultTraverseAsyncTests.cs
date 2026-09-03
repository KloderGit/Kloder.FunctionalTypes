using FunctionalTypes.TypedErrorResult;

namespace UnitTest;

[TestFixture]
public class TypedErrorResultTraverseAsyncTests
{
    [Test]
    public async Task TraverseAsync_AllSucceed_ProducesSuccessWithAllValues()
    {
        var source = new[] { 1, 2, 3 };

        Result<IEnumerable<int>, DomainError> result = await source.TraverseAsync(v =>
            Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(v * 2)));

        Assert.That(result.Match(v => v, _ => []), Is.EqualTo(new[] { 2, 4, 6 }));
    }

    [Test]
    public async Task TraverseAsync_OneFails_ShortCircuitsWithoutAwaitingRemainingItems()
    {
        var source = new[] { 1, 2, 3, 4 };
        var processed = new List<int>();

        Result<IEnumerable<int>, DomainError> result = await source.TraverseAsync(async v =>
        {
            processed.Add(v);
            await Task.Yield();
            return v == 2
                ? new Failure<int, DomainError>(DomainError.Invalid)
                : (Result<int, DomainError>)new Success<int, DomainError>(v);
        });

        Assert.That(result.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
        Assert.That(processed, Is.EqualTo(new[] { 1, 2 }));
    }

    [Test]
    public async Task SequenceAsync_AllSucceed_ProducesSuccessWithAllValues()
    {
        var source = new[]
        {
            Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(1)),
            Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(2)),
            Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(3))
        };

        Result<IEnumerable<int>, DomainError> result = await source.SequenceAsync();

        Assert.That(result.Match(v => v, _ => []), Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public async Task SequenceAsync_OneFails_ProducesFirstFailure()
    {
        var source = new[]
        {
            Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(1)),
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound)),
            Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(3))
        };

        Result<IEnumerable<int>, DomainError> result = await source.SequenceAsync();

        Assert.That(result.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}
