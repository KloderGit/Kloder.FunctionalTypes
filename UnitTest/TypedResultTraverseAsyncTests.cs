using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class TypedResultTraverseAsyncTests
{
    [Test]
    public async Task TraverseAsync_AllSucceed_ProducesSuccessWithAllValues()
    {
        var source = new[] { 1, 2, 3 };

        Result<IEnumerable<int>> result = await source.TraverseAsync(v => Task.FromResult<Result<int>>(new Success<int>(v * 2)));

        Assert.That(result.Match(v => v, _ => []), Is.EqualTo(new[] { 2, 4, 6 }));
    }

    [Test]
    public async Task TraverseAsync_OneFails_ShortCircuitsWithoutAwaitingRemainingItems()
    {
        var source = new[] { 1, 2, 3, 4 };
        var processed = new List<int>();

        Result<IEnumerable<int>> result = await source.TraverseAsync(async v =>
        {
            processed.Add(v);
            await Task.Yield();
            return v == 2 ? new Failure<int>("bad item") : (Result<int>)new Success<int>(v);
        });

        Assert.That(result.Match(_ => "success", e => e), Is.EqualTo("bad item"));
        Assert.That(processed, Is.EqualTo(new[] { 1, 2 }));
    }

    [Test]
    public async Task SequenceAsync_AllSucceed_ProducesSuccessWithAllValues()
    {
        var source = new[]
        {
            Task.FromResult<Result<int>>(new Success<int>(1)),
            Task.FromResult<Result<int>>(new Success<int>(2)),
            Task.FromResult<Result<int>>(new Success<int>(3))
        };

        Result<IEnumerable<int>> result = await source.SequenceAsync();

        Assert.That(result.Match(v => v, _ => []), Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public async Task SequenceAsync_OneFails_ProducesFirstFailure()
    {
        var source = new[]
        {
            Task.FromResult<Result<int>>(new Success<int>(1)),
            Task.FromResult<Result<int>>(new Failure<int>("boom")),
            Task.FromResult<Result<int>>(new Success<int>(3))
        };

        Result<IEnumerable<int>> result = await source.SequenceAsync();

        Assert.That(result.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }
}
