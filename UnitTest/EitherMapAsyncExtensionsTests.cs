using FunctionalTypes.Either;

namespace UnitTest;

[TestFixture]
public class EitherMapAsync_SyncEitherToTaskTests
{
    [Test]
    public async Task MapLeftAsync_OnLeft_TransformsLeftValue()
    {
        Either<int, string> either = Either<int, string>.Left(21);

        Either<string, string> mapped = await either.MapLeftAsync(v => Task.FromResult((v * 2).ToString()));

        Assert.That(mapped.Match(l => l, r => r), Is.EqualTo("42"));
        Assert.That(mapped.IsLeft, Is.True);
    }

    [Test]
    public async Task MapLeftAsync_OnRight_SkipsSelectorAndPassesThrough()
    {
        Either<int, string> either = Either<int, string>.Right("hello");
        var selectorWasCalled = false;

        Either<string, string> mapped = await either.MapLeftAsync<int, string, string>(v =>
        {
            selectorWasCalled = true;
            return Task.FromResult(v.ToString());
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(l => l, r => r), Is.EqualTo("hello"));
        Assert.That(mapped.IsRight, Is.True);
    }

    [Test]
    public async Task MapRightAsync_OnRight_TransformsRightValue()
    {
        Either<string, int> either = Either<string, int>.Right(21);

        Either<string, string> mapped = await either.MapRightAsync(v => Task.FromResult((v * 2).ToString()));

        Assert.That(mapped.Match(l => l, r => r), Is.EqualTo("42"));
        Assert.That(mapped.IsRight, Is.True);
    }

    [Test]
    public async Task MapRightAsync_OnLeft_SkipsSelectorAndPassesThrough()
    {
        Either<string, int> either = Either<string, int>.Left("hello");
        var selectorWasCalled = false;

        Either<string, string> mapped = await either.MapRightAsync<string, int, string>(v =>
        {
            selectorWasCalled = true;
            return Task.FromResult(v.ToString());
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.Match(l => l, r => r), Is.EqualTo("hello"));
        Assert.That(mapped.IsLeft, Is.True);
    }
}

[TestFixture]
public class EitherMapAsync_TaskToSyncSelectorTests
{
    [Test]
    public async Task MapLeft_OnLeft_TransformsLeftValue()
    {
        Task<Either<int, string>> eitherTask = Task.FromResult(Either<int, string>.Left(21));

        Either<string, string> mapped = await eitherTask.MapLeft(v => (v * 2).ToString());

        Assert.That(mapped.Match(l => l, r => r), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapRight_OnRight_TransformsRightValue()
    {
        Task<Either<string, int>> eitherTask = Task.FromResult(Either<string, int>.Right(21));

        Either<string, string> mapped = await eitherTask.MapRight(v => (v * 2).ToString());

        Assert.That(mapped.Match(l => l, r => r), Is.EqualTo("42"));
    }
}

[TestFixture]
public class EitherMapAsync_TaskToTaskTests
{
    [Test]
    public async Task MapLeftAsync_OnLeft_TransformsLeftValue()
    {
        Task<Either<int, string>> eitherTask = Task.FromResult(Either<int, string>.Left(21));

        Either<string, string> mapped = await eitherTask.MapLeftAsync(v => Task.FromResult((v * 2).ToString()));

        Assert.That(mapped.Match(l => l, r => r), Is.EqualTo("42"));
    }

    [Test]
    public async Task MapRightAsync_OnRight_TransformsRightValue()
    {
        Task<Either<string, int>> eitherTask = Task.FromResult(Either<string, int>.Right(21));

        Either<string, string> mapped = await eitherTask.MapRightAsync(v => Task.FromResult((v * 2).ToString()));

        Assert.That(mapped.Match(l => l, r => r), Is.EqualTo("42"));
    }
}
