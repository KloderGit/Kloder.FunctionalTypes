using FunctionalTypes.Either;
using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class EitherBindAsync_SyncEitherToTaskTests
{
    [Test]
    public async Task BindLeftAsync_OnLeft_RunsBinder()
    {
        Either<int, string> either = Either<int, string>.Left(21);

        Either<string, string> bound = await either.BindLeftAsync(v =>
            Task.FromResult(Either<string, string>.Left((v * 2).ToString())));

        Assert.That(bound.Match(l => l, r => r), Is.EqualTo("42"));
        Assert.That(bound.IsLeft, Is.True);
    }

    [Test]
    public async Task BindLeftAsync_OnRight_SkipsBinderAndPassesThrough()
    {
        Either<int, string> either = Either<int, string>.Right("hello");
        var binderWasCalled = false;

        Either<string, string> bound = await either.BindLeftAsync<int, string, string>(v =>
        {
            binderWasCalled = true;
            return Task.FromResult(Either<string, string>.Left(v.ToString()));
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(l => l, r => r), Is.EqualTo("hello"));
        Assert.That(bound.IsRight, Is.True);
    }

    [Test]
    public async Task BindRightAsync_OnRight_RunsBinder()
    {
        Either<string, int> either = Either<string, int>.Right(21);

        Either<string, string> bound = await either.BindRightAsync(v =>
            Task.FromResult(Either<string, string>.Right((v * 2).ToString())));

        Assert.That(bound.Match(l => l, r => r), Is.EqualTo("42"));
        Assert.That(bound.IsRight, Is.True);
    }

    [Test]
    public async Task BindRightAsync_OnLeft_SkipsBinderAndPassesThrough()
    {
        Either<string, int> either = Either<string, int>.Left("hello");
        var binderWasCalled = false;

        Either<string, string> bound = await either.BindRightAsync<string, int, string>(v =>
        {
            binderWasCalled = true;
            return Task.FromResult(Either<string, string>.Right(v.ToString()));
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(l => l, r => r), Is.EqualTo("hello"));
        Assert.That(bound.IsLeft, Is.True);
    }
}

[TestFixture]
public class EitherBindAsync_TaskToSyncBinderTests
{
    [Test]
    public async Task BindLeft_OnLeft_RunsBinder()
    {
        Task<Either<int, string>> eitherTask = Task.FromResult(Either<int, string>.Left(21));

        Either<string, string> bound = await eitherTask.BindLeft(v => Either<string, string>.Left((v * 2).ToString()));

        Assert.That(bound.Match(l => l, r => r), Is.EqualTo("42"));
    }

    [Test]
    public async Task BindRight_OnRight_RunsBinder()
    {
        Task<Either<string, int>> eitherTask = Task.FromResult(Either<string, int>.Right(21));

        Either<string, string> bound = await eitherTask.BindRight(v => Either<string, string>.Right((v * 2).ToString()));

        Assert.That(bound.Match(l => l, r => r), Is.EqualTo("42"));
    }
}

[TestFixture]
public class EitherBindAsync_TaskToTaskTests
{
    [Test]
    public async Task BindLeftAsync_OnLeft_RunsBinder()
    {
        Task<Either<int, string>> eitherTask = Task.FromResult(Either<int, string>.Left(21));

        Either<string, string> bound = await eitherTask.BindLeftAsync(v =>
            Task.FromResult(Either<string, string>.Left((v * 2).ToString())));

        Assert.That(bound.Match(l => l, r => r), Is.EqualTo("42"));
    }

    [Test]
    public async Task BindRightAsync_OnRight_RunsBinder()
    {
        Task<Either<string, int>> eitherTask = Task.FromResult(Either<string, int>.Right(21));

        Either<string, string> bound = await eitherTask.BindRightAsync(v =>
            Task.FromResult(Either<string, string>.Right((v * 2).ToString())));

        Assert.That(bound.Match(l => l, r => r), Is.EqualTo("42"));
    }
}

[TestFixture]
public class EitherBindAsync_ResultBridgeTests
{
    [Test]
    public async Task BindAsync_OnSyncResult_LeftBranch_RunsOnLeftBinder()
    {
        Result<Either<int, string>> result = new Success<Either<int, string>>(Either<int, string>.Left(21));

        Result<string> bound = await result.BindAsync(
            onLeft: v => Task.FromResult<Result<string>>(new Success<string>((v * 2).ToString())),
            onRight: v => Task.FromResult<Result<string>>(new Success<string>(v)));

        Assert.That(bound.Match(v => v, e => e), Is.EqualTo("42"));
    }

    [Test]
    public async Task BindAsync_OnSyncResult_RightBranch_RunsOnRightBinder()
    {
        Result<Either<int, string>> result = new Success<Either<int, string>>(Either<int, string>.Right("hello"));

        Result<string> bound = await result.BindAsync(
            onLeft: v => Task.FromResult<Result<string>>(new Success<string>(v.ToString())),
            onRight: v => Task.FromResult<Result<string>>(new Success<string>(v)));

        Assert.That(bound.Match(v => v, e => e), Is.EqualTo("hello"));
    }

    [Test]
    public async Task BindAsync_OnFailure_ShortCircuitsWithoutRunningEitherBranch()
    {
        Result<Either<int, string>> result = new Failure<Either<int, string>>("boom");
        var onLeftWasCalled = false;
        var onRightWasCalled = false;

        Result<string> bound = await result.BindAsync(
            onLeft: v =>
            {
                onLeftWasCalled = true;
                return Task.FromResult<Result<string>>(new Success<string>(v.ToString()));
            },
            onRight: v =>
            {
                onRightWasCalled = true;
                return Task.FromResult<Result<string>>(new Success<string>(v));
            });

        Assert.That(onLeftWasCalled, Is.False);
        Assert.That(onRightWasCalled, Is.False);
        Assert.That(bound.Match(v => v, e => e), Is.EqualTo("boom"));
    }

    [Test]
    public async Task Bind_OnTaskResult_LeftBranch_RunsOnLeftBinder()
    {
        Task<Result<Either<int, string>>> resultTask =
            Task.FromResult<Result<Either<int, string>>>(new Success<Either<int, string>>(Either<int, string>.Left(21)));

        Result<string> bound = await resultTask.Bind(
            onLeft: v => new Success<string>((v * 2).ToString()),
            onRight: v => new Success<string>(v));

        Assert.That(bound.Match(v => v, e => e), Is.EqualTo("42"));
    }

    [Test]
    public async Task BindAsync_OnTaskResult_RightBranch_RunsOnRightBinder()
    {
        Task<Result<Either<int, string>>> resultTask =
            Task.FromResult<Result<Either<int, string>>>(new Success<Either<int, string>>(Either<int, string>.Right("hello")));

        Result<string> bound = await resultTask.BindAsync(
            onLeft: v => Task.FromResult<Result<string>>(new Success<string>(v.ToString())),
            onRight: v => Task.FromResult<Result<string>>(new Success<string>(v)));

        Assert.That(bound.Match(v => v, e => e), Is.EqualTo("hello"));
    }
}
