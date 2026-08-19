using FunctionalTypes.SimpleResult;
using FunctionalTypes.TypedErrorResult;
using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class SimpleResultApplicativeExtensions_FuncToTypedResultTests
{
    [Test]
    public void Func_OnSuccess_WrapsFunctionInSuccess()
    {
        Result result = new Success();

        var wrapped = result.Func((int x) => x.ToString());

        Assert.That(wrapped.Match(f => f(42), e => e), Is.EqualTo("42"));
    }

    [Test]
    public void Func_OnFailure_PropagatesErrorWithoutTouchingFunc()
    {
        Result result = new Failure("boom");
        var funcWasCalled = false;

        var wrapped = result.Func((int x) =>
        {
            funcWasCalled = true;
            return x.ToString();
        });

        Assert.That(funcWasCalled, Is.False);
        Assert.That(wrapped.Match(f => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public void Func_AcceptsCurriedLambdaWithoutExplicitTypeArguments()
    {
        Result result = new Success();

        // TFunc is a single type parameter, inferred from the lambda's natural type —
        // no `.Func<int, Func<int, int>>` needed, as long as every parameter is explicitly typed.
        var wrapped = result.Func((int a) => (int b) => a + b);

        Assert.That(wrapped.Match(f => f(1)(2), e => -1), Is.EqualTo(3));
    }
}

[TestFixture]
public class SimpleResultApplicativeExtensions_FuncToTypedErrorResultTests
{
    [Test]
    public void Func_OnSuccess_WrapsFunctionInSuccess()
    {
        Result result = new Success();

        var wrapped = result.Func((int x) => x.ToString(), _ => DomainError.Invalid);

        Assert.That(wrapped.Match(f => f(42), _ => "failure"), Is.EqualTo("42"));
    }

    [Test]
    public void Func_OnFailure_ConvertsErrorAndSkipsFunc()
    {
        Result result = new Failure("not-found");
        var funcWasCalled = false;

        var wrapped = result.Func(
            (int x) =>
            {
                funcWasCalled = true;
                return x.ToString();
            },
            errorSelector: msg => msg == "not-found" ? DomainError.NotFound : DomainError.Invalid);

        Assert.That(funcWasCalled, Is.False);
        Assert.That(wrapped.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}
