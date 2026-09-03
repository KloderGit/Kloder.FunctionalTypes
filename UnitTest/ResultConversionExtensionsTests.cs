using FunctionalTypes.SimpleResult;
using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class ResultConversionExtensionsTests
{
    [Test]
    public void ToResult_WrapsValueInSuccess()
    {
        Result<int> result = 42.ToResult();

        Assert.That(result.Match(v => v, e => -1), Is.EqualTo(42));
    }
}

[TestFixture]
public class BoolExtensionsTests
{
    [Test]
    public void Then_True_RunsFactoryAndProducesSuccess()
    {
        Result<int> result = true.Then(() => 42, "invalid");

        Assert.That(result.Match(v => v, e => -1), Is.EqualTo(42));
    }

    [Test]
    public void Then_False_SkipsFactoryAndProducesFailure()
    {
        var factoryWasCalled = false;

        Result<int> result = false.Then(() =>
        {
            factoryWasCalled = true;
            return 42;
        }, "invalid");

        Assert.That(factoryWasCalled, Is.False);
        Assert.That(result.Match(_ => "success", e => e), Is.EqualTo("invalid"));
    }

    [Test]
    public void ToResult_True_ProducesSuccess()
    {
        Result result = true.ToResult("invalid");

        Assert.That(result.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public void ToResult_False_ProducesFailureWithMessage()
    {
        Result result = false.ToResult("invalid");

        Assert.That(result.Match(() => "success", e => e), Is.EqualTo("invalid"));
    }
}
