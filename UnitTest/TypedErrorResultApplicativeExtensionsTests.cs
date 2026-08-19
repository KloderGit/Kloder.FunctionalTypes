using FunctionalTypes.SimpleResult;
using FunctionalTypes.TypedErrorResult;
using static FunctionalTypes.TypedErrorResult.ApplicativeExtensions;

namespace UnitTest;

[TestFixture]
public class TypedErrorResultApplicativeExtensions_ApplyTests
{
    [Test]
    public void Apply_FuncSuccess_ArgSuccess_ReturnsAppliedResult()
    {
        Result<Func<int, int>, DomainError> func = new Success<Func<int, int>, DomainError>(x => x * 2);
        Result<int, DomainError> arg = new Success<int, DomainError>(21);

        Result<int, DomainError> applied = func.Apply(arg);

        Assert.That(applied.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public void Apply_FuncFailure_ArgSuccess_ShortCircuitsWithFuncError()
    {
        Result<Func<int, int>, DomainError> func = new Failure<Func<int, int>, DomainError>(DomainError.Invalid);
        Result<int, DomainError> arg = new Success<int, DomainError>(21);

        Result<int, DomainError> applied = func.Apply(arg);

        Assert.That(applied.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }

    [Test]
    public void Apply_FuncSuccess_ArgFailure_ShortCircuitsWithArgError()
    {
        Result<Func<int, int>, DomainError> func = new Success<Func<int, int>, DomainError>(x => x * 2);
        Result<int, DomainError> arg = new Failure<int, DomainError>(DomainError.NotFound);

        Result<int, DomainError> applied = func.Apply(arg);

        Assert.That(applied.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }

    [Test]
    public void Apply_FuncFailure_ArgFailure_ReturnsFuncErrorOnly()
    {
        Result<Func<int, int>, DomainError> func = new Failure<Func<int, int>, DomainError>(DomainError.Invalid);
        Result<int, DomainError> arg = new Failure<int, DomainError>(DomainError.NotFound);

        Result<int, DomainError> applied = func.Apply(arg);

        Assert.That(applied.Match(_ => "success", e => e.ToString()), Is.EqualTo(DomainError.Invalid.ToString()));
    }
}

[TestFixture]
public class TypedErrorResultApplicativeExtensions_GateFuncChainedUsageTests
{
    [Test]
    public void FuncThenApply_ChainsTwoArguments_AllSuccess()
    {
        Result start = new Success();

        var curried = start.Func((int a) => (int b) => a + b, errorSelector: _ => DomainError.Invalid);

        Result<int, DomainError> sum = curried
            .Apply(new Success<int, DomainError>(1))
            .Apply(new Success<int, DomainError>(2));

        Assert.That(sum.Match(v => v, _ => -1), Is.EqualTo(3));
    }

    [Test]
    public void FuncThenApply_ChainsTwoArguments_FirstArgFails()
    {
        Result start = new Success();

        var curried = start.Func((int a) => (int b) => a + b, errorSelector: _ => DomainError.Invalid);

        Result<int, DomainError> sum = curried
            .Apply(new Failure<int, DomainError>(DomainError.NotFound))
            .Apply(new Success<int, DomainError>(2));

        Assert.That(sum.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }

    [Test]
    public void FuncThenApply_StartingResultIsFailure_ConvertsErrorAndShortCircuitsBeforeAnyApply()
    {
        Result start = new Failure("not-found");

        var wrapped = start.Func(
            (int a) => a + 1,
            errorSelector: msg => msg == "not-found" ? DomainError.NotFound : DomainError.Invalid);

        Result<int, DomainError> applied = wrapped.Apply(new Success<int, DomainError>(1));

        Assert.That(applied.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}

[TestFixture]
public class TypedErrorResultApplicativeExtensions_PureFuncEmbeddedInFlowTests
{
    [Test]
    public void Func_NoReceiverNeeded_LiftsCurriedFunctionMidFlow()
    {
        // TError has no argument to infer it from, so — unlike the TypedResult version —
        // both type arguments must be given explicitly here.
        Result<int, DomainError> sum = Func<Func<int, Func<int, int>>, DomainError>(a => b => a + b)
            .Apply(new Success<int, DomainError>(1))
            .Apply(new Success<int, DomainError>(2));

        Assert.That(sum.Match(v => v, _ => -1), Is.EqualTo(3));
    }

    [Test]
    public void Func_EmbeddedInsideBindContinuation_ArgFailurePropagates()
    {
        Result<int, DomainError> seed = new Success<int, DomainError>(10);

        Result<int, DomainError> sum = seed.Bind(seedValue =>
            Func<Func<int, Func<int, int>>, DomainError>(a => b => seedValue + a + b)
                .Apply(new Failure<int, DomainError>(DomainError.Invalid))
                .Apply(new Success<int, DomainError>(2)));

        Assert.That(sum.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}
