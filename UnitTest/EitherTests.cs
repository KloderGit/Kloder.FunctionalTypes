using FunctionalTypes.Either;

namespace UnitTest;

[TestFixture]
public class EitherTests
{
    [Test]
    public void Left_IsLeft_IsTrue_IsRight_IsFalse()
    {
        Either<int, string> either = Either<int, string>.Left(42);

        Assert.That(either.IsLeft, Is.True);
        Assert.That(either.IsRight, Is.False);
    }

    [Test]
    public void Right_IsRight_IsTrue_IsLeft_IsFalse()
    {
        Either<int, string> either = Either<int, string>.Right("ok");

        Assert.That(either.IsLeft, Is.False);
        Assert.That(either.IsRight, Is.True);
    }

    [Test]
    public void Match_OnLeft_RunsOnLeftBranch()
    {
        Either<int, string> either = Either<int, string>.Left(42);

        var result = either.Match(l => $"left={l}", r => $"right={r}");

        Assert.That(result, Is.EqualTo("left=42"));
    }

    [Test]
    public void Match_OnRight_RunsOnRightBranch()
    {
        Either<int, string> either = Either<int, string>.Right("ok");

        var result = either.Match(l => $"left={l}", r => $"right={r}");

        Assert.That(result, Is.EqualTo("right=ok"));
    }

    [Test]
    public void MapLeft_OnLeft_TransformsValue()
    {
        Either<int, string> either = Either<int, string>.Left(42);

        Either<string, string> mapped = either.MapLeft(l => $"left={l}");

        Assert.That(mapped.Match(l => l, r => r), Is.EqualTo("left=42"));
    }

    [Test]
    public void MapLeft_OnRight_PassesThroughUnchanged()
    {
        Either<int, string> either = Either<int, string>.Right("ok");

        Either<string, string> mapped = either.MapLeft(l => $"left={l}");

        Assert.That(mapped.Match(l => l, r => r), Is.EqualTo("ok"));
    }

    [Test]
    public void MapRight_OnRight_TransformsValue()
    {
        Either<int, string> either = Either<int, string>.Right("ok");

        Either<int, string> mapped = either.MapRight(r => $"right={r}");

        Assert.That(mapped.Match(l => $"left={l}", r => r), Is.EqualTo("right=ok"));
    }

    [Test]
    public void MapRight_OnLeft_PassesThroughUnchanged()
    {
        Either<int, string> either = Either<int, string>.Left(42);

        Either<int, string> mapped = either.MapRight(r => $"right={r}");

        Assert.That(mapped.Match(l => l, r => 0), Is.EqualTo(42));
    }

    [Test]
    public void BindLeft_OnLeft_RunsBinder()
    {
        Either<int, string> either = Either<int, string>.Left(42);

        Either<string, string> bound = either.BindLeft(l => Either<string, string>.Left($"left={l}"));

        Assert.That(bound.Match(l => l, r => r), Is.EqualTo("left=42"));
    }

    [Test]
    public void BindLeft_OnRight_ShortCircuits()
    {
        Either<int, string> either = Either<int, string>.Right("ok");
        var binderWasCalled = false;

        Either<string, string> bound = either.BindLeft(l =>
        {
            binderWasCalled = true;
            return Either<string, string>.Left($"left={l}");
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(l => l, r => r), Is.EqualTo("ok"));
    }

    [Test]
    public void BindRight_OnRight_RunsBinder()
    {
        Either<int, string> either = Either<int, string>.Right("ok");

        Either<int, string> bound = either.BindRight(r => Either<int, string>.Right($"right={r}"));

        Assert.That(bound.Match(l => $"left={l}", r => r), Is.EqualTo("right=ok"));
    }

    [Test]
    public void BindRight_OnLeft_ShortCircuits()
    {
        Either<int, string> either = Either<int, string>.Left(42);
        var binderWasCalled = false;

        Either<int, string> bound = either.BindRight(r =>
        {
            binderWasCalled = true;
            return Either<int, string>.Right($"right={r}");
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(l => l, r => 0), Is.EqualTo(42));
    }

    [Test]
    public void TapLeft_OnLeft_RunsAction()
    {
        Either<int, string> either = Either<int, string>.Left(42);
        var tapped = 0;

        either.TapLeft(l => tapped = l);

        Assert.That(tapped, Is.EqualTo(42));
    }

    [Test]
    public void TapLeft_OnRight_DoesNotRunAction()
    {
        Either<int, string> either = Either<int, string>.Right("ok");
        var tappedCalled = false;

        either.TapLeft(_ => tappedCalled = true);

        Assert.That(tappedCalled, Is.False);
    }

    [Test]
    public void TapRight_OnRight_RunsAction()
    {
        Either<int, string> either = Either<int, string>.Right("ok");
        string? tapped = null;

        either.TapRight(r => tapped = r);

        Assert.That(tapped, Is.EqualTo("ok"));
    }

    [Test]
    public void TapRight_OnLeft_DoesNotRunAction()
    {
        Either<int, string> either = Either<int, string>.Left(42);
        var tappedCalled = false;

        either.TapRight(_ => tappedCalled = true);

        Assert.That(tappedCalled, Is.False);
    }

    [Test]
    public void Swap_OnLeft_BecomesRight()
    {
        Either<int, string> either = Either<int, string>.Left(42);

        Either<string, int> swapped = either.Swap();

        Assert.That(swapped.IsRight, Is.True);
        Assert.That(swapped.Match(l => -1, r => r), Is.EqualTo(42));
    }

    [Test]
    public void Swap_OnRight_BecomesLeft()
    {
        Either<int, string> either = Either<int, string>.Right("ok");

        Either<string, int> swapped = either.Swap();

        Assert.That(swapped.IsLeft, Is.True);
        Assert.That(swapped.Match(l => l, r => "n/a"), Is.EqualTo("ok"));
    }

    [Test]
    public void Deconstruct_OnLeft_ReturnsLeftValue()
    {
        Either<int, string> either = Either<int, string>.Left(42);

        var (isLeft, left, right) = either;

        Assert.That(isLeft, Is.True);
        Assert.That(left, Is.EqualTo(42));
        Assert.That(right, Is.Null);
    }

    [Test]
    public void Deconstruct_OnRight_ReturnsRightValue()
    {
        Either<int, string> either = Either<int, string>.Right("ok");

        var (isLeft, left, right) = either;

        Assert.That(isLeft, Is.False);
        Assert.That(left, Is.EqualTo(0));
        Assert.That(right, Is.EqualTo("ok"));
    }
}
