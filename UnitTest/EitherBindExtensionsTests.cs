using FunctionalTypes.Either;
using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class EitherBindExtensionsTests
{
    [Test]
    public void Bind_TwoBranches_OnLeft_RunsOnLeftBinder()
    {
        Result<Either<int, string>> result = new Success<Either<int, string>>(Either<int, string>.Left(42));

        Result<string> bound = result.Bind(
            onLeft: l => new Success<string>($"left={l}"),
            onRight: r => new Success<string>($"right={r}"));

        Assert.That(bound.Match(v => v, e => e), Is.EqualTo("left=42"));
    }

    [Test]
    public void Bind_TwoBranches_OnRight_RunsOnRightBinder()
    {
        Result<Either<int, string>> result = new Success<Either<int, string>>(Either<int, string>.Right("ok"));

        Result<string> bound = result.Bind(
            onLeft: l => new Success<string>($"left={l}"),
            onRight: r => new Success<string>($"right={r}"));

        Assert.That(bound.Match(v => v, e => e), Is.EqualTo("right=ok"));
    }

    [Test]
    public void Bind_TwoBranches_OnFailure_ShortCircuitsWithoutRunningEitherBinder()
    {
        Result<Either<int, string>> result = new Failure<Either<int, string>>("boom");
        var onLeftCalled = false;
        var onRightCalled = false;

        Result<string> bound = result.Bind(
            onLeft: l =>
            {
                onLeftCalled = true;
                return new Success<string>($"left={l}");
            },
            onRight: r =>
            {
                onRightCalled = true;
                return new Success<string>($"right={r}");
            });

        Assert.That(onLeftCalled, Is.False);
        Assert.That(onRightCalled, Is.False);
        Assert.That(bound.Match(v => v, e => e), Is.EqualTo("boom"));
    }

    [Test]
    public void Collapse_OnLeft_ReturnsLeftValue()
    {
        Result<Either<int, int>> result = new Success<Either<int, int>>(Either<int, int>.Left(42));

        Result<string> bound = result.Collapse().Bind(v => new Success<string>($"value={v}"));

        Assert.That(bound.Match(v => v, e => e), Is.EqualTo("value=42"));
    }

    [Test]
    public void Collapse_OnRight_ReturnsRightValue()
    {
        Result<Either<int, int>> result = new Success<Either<int, int>>(Either<int, int>.Right(7));

        Result<string> bound = result.Collapse().Bind(v => new Success<string>($"value={v}"));

        Assert.That(bound.Match(v => v, e => e), Is.EqualTo("value=7"));
    }

    [Test]
    public void Collapse_OnFailure_ShortCircuitsWithoutRunningBinder()
    {
        Result<Either<int, int>> result = new Failure<Either<int, int>>("boom");
        var binderWasCalled = false;

        Result<string> bound = result.Collapse().Bind(v =>
        {
            binderWasCalled = true;
            return new Success<string>($"value={v}");
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(v => v, e => e), Is.EqualTo("boom"));
    }
}
