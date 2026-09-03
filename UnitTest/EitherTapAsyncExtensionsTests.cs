using FunctionalTypes.Either;

namespace UnitTest;

[TestFixture]
public class EitherTapAsync_SyncEitherToTaskTests
{
    [Test]
    public async Task TapLeftAsync_OnLeft_RunsActionAndReturnsUnchangedEither()
    {
        Either<int, string> either = Either<int, string>.Left(21);
        var seen = -1;

        Either<int, string> tapped = await either.TapLeftAsync(v =>
        {
            seen = v;
            return Task.CompletedTask;
        });

        Assert.That(seen, Is.EqualTo(21));
        Assert.That(tapped, Is.SameAs(either));
    }

    [Test]
    public async Task TapLeftAsync_OnRight_SkipsAction()
    {
        Either<int, string> either = Either<int, string>.Right("hello");
        var tapWasCalled = false;

        Either<int, string> tapped = await either.TapLeftAsync(_ =>
        {
            tapWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(either));
    }

    [Test]
    public async Task TapRightAsync_OnRight_RunsActionAndReturnsUnchangedEither()
    {
        Either<string, int> either = Either<string, int>.Right(21);
        var seen = -1;

        Either<string, int> tapped = await either.TapRightAsync(v =>
        {
            seen = v;
            return Task.CompletedTask;
        });

        Assert.That(seen, Is.EqualTo(21));
        Assert.That(tapped, Is.SameAs(either));
    }

    [Test]
    public async Task TapRightAsync_OnLeft_SkipsAction()
    {
        Either<string, int> either = Either<string, int>.Left("hello");
        var tapWasCalled = false;

        Either<string, int> tapped = await either.TapRightAsync(_ =>
        {
            tapWasCalled = true;
            return Task.CompletedTask;
        });

        Assert.That(tapWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(either));
    }
}

[TestFixture]
public class EitherTapAsync_TaskToSyncActionTests
{
    [Test]
    public async Task TapLeft_OnLeft_RunsAction()
    {
        Task<Either<int, string>> eitherTask = Task.FromResult(Either<int, string>.Left(21));
        var seen = -1;

        Either<int, string> tapped = await eitherTask.TapLeft(v => seen = v);

        Assert.That(seen, Is.EqualTo(21));
        Assert.That(tapped.Match(l => l.ToString(), r => r), Is.EqualTo("21"));
    }

    [Test]
    public async Task TapRight_OnRight_RunsAction()
    {
        Task<Either<string, int>> eitherTask = Task.FromResult(Either<string, int>.Right(21));
        var seen = -1;

        Either<string, int> tapped = await eitherTask.TapRight(v => seen = v);

        Assert.That(seen, Is.EqualTo(21));
        Assert.That(tapped.Match(l => l, r => r.ToString()), Is.EqualTo("21"));
    }
}

[TestFixture]
public class EitherTapAsync_TaskToTaskTests
{
    [Test]
    public async Task TapLeftAsync_OnLeft_RunsAction()
    {
        Task<Either<int, string>> eitherTask = Task.FromResult(Either<int, string>.Left(21));
        var seen = -1;

        Either<int, string> tapped = await eitherTask.TapLeftAsync(v =>
        {
            seen = v;
            return Task.CompletedTask;
        });

        Assert.That(seen, Is.EqualTo(21));
        Assert.That(tapped.Match(l => l.ToString(), r => r), Is.EqualTo("21"));
    }

    [Test]
    public async Task TapRightAsync_OnRight_RunsAction()
    {
        Task<Either<string, int>> eitherTask = Task.FromResult(Either<string, int>.Right(21));
        var seen = -1;

        Either<string, int> tapped = await eitherTask.TapRightAsync(v =>
        {
            seen = v;
            return Task.CompletedTask;
        });

        Assert.That(seen, Is.EqualTo(21));
        Assert.That(tapped.Match(l => l, r => r.ToString()), Is.EqualTo("21"));
    }
}
