using FunctionalTypes.SimpleResult;
using FunctionalTypes.TypedResult;
using static FunctionalTypes.TypedResult.ApplicativeExtensions;

namespace UnitTest;

[TestFixture]
public class TypedResultApplicativeExtensions_ApplyTests
{
    [Test]
    public void Apply_FuncSuccess_ArgSuccess_ReturnsAppliedResult()
    {
        Result<Func<int, int>> func = new Success<Func<int, int>>(x => x * 2);
        Result<int> arg = new Success<int>(21);

        Result<int> applied = func.Apply(arg);

        Assert.That(applied.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public void Apply_FuncFailure_ArgSuccess_ShortCircuitsWithFuncError()
    {
        Result<Func<int, int>> func = new Failure<Func<int, int>>("func-failed");
        Result<int> arg = new Success<int>(21);

        Result<int> applied = func.Apply(arg);

        Assert.That(applied.Match(v => "success", e => e), Is.EqualTo("func-failed"));
    }

    [Test]
    public void Apply_FuncSuccess_ArgFailure_ShortCircuitsWithArgError()
    {
        Result<Func<int, int>> func = new Success<Func<int, int>>(x => x * 2);
        Result<int> arg = new Failure<int>("arg-failed");

        Result<int> applied = func.Apply(arg);

        Assert.That(applied.Match(v => "success", e => e), Is.EqualTo("arg-failed"));
    }

    [Test]
    public void Apply_FuncFailure_ArgFailure_ReturnsFuncErrorOnly()
    {
        Result<Func<int, int>> func = new Failure<Func<int, int>>("func-failed");
        Result<int> arg = new Failure<int>("arg-failed");

        Result<int> applied = func.Apply(arg);

        Assert.That(applied.Match(v => "success", e => e), Is.EqualTo("func-failed"));
    }
}

[TestFixture]
public class TypedResultApplicativeExtensions_GateFuncChainedUsageTests
{
    [Test]
    public void FuncThenApply_ChainsTwoArguments_AllSuccess()
    {
        Result start = new Success();

        var curried = start.Func((int a) => (int b) => a + b);

        Result<int> sum = curried
            .Apply(new Success<int>(1))
            .Apply(new Success<int>(2));

        Assert.That(sum.Match(v => v, _ => -1), Is.EqualTo(3));
    }

    [Test]
    public void FuncThenApply_ChainsTwoArguments_FirstArgFails()
    {
        Result start = new Success();

        var curried = start.Func((int a) => (int b) => a + b);

        Result<int> sum = curried
            .Apply(new Failure<int>("first-failed"))
            .Apply(new Success<int>(2));

        Assert.That(sum.Match(v => "success", e => e), Is.EqualTo("first-failed"));
    }

    [Test]
    public void FuncThenApply_StartingResultIsFailure_ShortCircuitsBeforeAnyApply()
    {
        Result start = new Failure("start-failed");

        var wrapped = start.Func((int a) => a + 1);
        Result<int> applied = wrapped.Apply(new Success<int>(1));

        Assert.That(applied.Match(v => "success", e => e), Is.EqualTo("start-failed"));
    }

    [Test]
    public void MapThenApply_StartsChainFromAlreadyTypedFirstArgument_AllSuccess()
    {
        Result<Func<int, int>> curried = new Success<int>(1)
            .Map<Func<int, int>>(a => b => a + b);

        Result<int> sum = curried.Apply(new Success<int>(2));

        Assert.That(sum.Match(v => v, _ => -1), Is.EqualTo(3));
    }
}

[TestFixture]
public class TypedResultApplicativeExtensions_PureFuncEmbeddedInFlowTests
{
    [Test]
    public void Func_NoReceiverNeeded_LiftsCurriedFunctionMidFlow()
    {
        // ApplicativeExtensions.Func has no `this Result` receiver — it can be called
        // anywhere in a flow (e.g. inside a Bind continuation), not just as a chain-starter.
        Result<int> sum = Func((int a) => (int b) => a + b)
            .Apply(new Success<int>(1))
            .Apply(new Success<int>(2));

        Assert.That(sum.Match(v => v, _ => -1), Is.EqualTo(3));
    }

    [Test]
    public void Func_EmbeddedInsideBindContinuation_AllSuccess()
    {
        Result<int> seed = new Success<int>(10);

        Result<int> sum = seed.Bind(seedValue =>
            Func((int a) => (int b) => seedValue + a + b)
                .Apply(new Success<int>(1))
                .Apply(new Success<int>(2)));

        Assert.That(sum.Match(v => v, _ => -1), Is.EqualTo(13));
    }

    [Test]
    public void Func_EmbeddedInsideBindContinuation_ArgFailurePropagates()
    {
        Result<int> seed = new Success<int>(10);

        Result<int> sum = seed.Bind(seedValue =>
            Func((int a) => (int b) => seedValue + a + b)
                .Apply(new Failure<int>("bad-arg"))
                .Apply(new Success<int>(2)));

        Assert.That(sum.Match(v => "success", e => e), Is.EqualTo("bad-arg"));
    }
}
