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

[TestFixture]
public class TypedErrorResultApplicativeExtensions_ApplyAsync_SyncFuncAsyncArgTests
{
    [Test]
    public async Task ApplyAsync_FuncSuccess_ArgSuccess_ReturnsAppliedResult()
    {
        Result<Func<int, int>, DomainError> func = new Success<Func<int, int>, DomainError>(x => x * 2);

        Result<int, DomainError> applied = await func.ApplyAsync(
            () => Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(21)));

        Assert.That(applied.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task ApplyAsync_FuncFailure_ArgFactoryIsNeverInvoked()
    {
        Result<Func<int, int>, DomainError> func = new Failure<Func<int, int>, DomainError>(DomainError.Invalid);
        var argFactoryWasCalled = false;

        Result<int, DomainError> applied = await func.ApplyAsync(() =>
        {
            argFactoryWasCalled = true;
            return Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(21));
        });

        Assert.That(argFactoryWasCalled, Is.False);
        Assert.That(applied.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }

    [Test]
    public async Task ApplyAsync_FuncSuccess_ArgFactoryFails_PropagatesArgError()
    {
        Result<Func<int, int>, DomainError> func = new Success<Func<int, int>, DomainError>(x => x * 2);

        Result<int, DomainError> applied = await func.ApplyAsync(
            () => Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound)));

        Assert.That(applied.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }
}

[TestFixture]
public class TypedErrorResultApplicativeExtensions_Apply_AsyncFuncSyncArgTests
{
    [Test]
    public async Task Apply_FuncTaskSuccess_ArgSuccess_ReturnsAppliedResult()
    {
        Task<Result<Func<int, int>, DomainError>> funcTask =
            Task.FromResult<Result<Func<int, int>, DomainError>>(new Success<Func<int, int>, DomainError>(x => x * 2));

        Result<int, DomainError> applied = await funcTask.Apply(new Success<int, DomainError>(21));

        Assert.That(applied.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task Apply_FuncTaskFailure_ArgSuccess_ShortCircuitsWithFuncError()
    {
        Task<Result<Func<int, int>, DomainError>> funcTask =
            Task.FromResult<Result<Func<int, int>, DomainError>>(new Failure<Func<int, int>, DomainError>(DomainError.Invalid));

        Result<int, DomainError> applied = await funcTask.Apply(new Success<int, DomainError>(21));

        Assert.That(applied.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}

[TestFixture]
public class TypedErrorResultApplicativeExtensions_ApplyAsync_AsyncFuncAsyncArgTests
{
    [Test]
    public async Task ApplyAsync_BothSuccess_AwaitsFuncBeforeInvokingArgFactory_Sequentially()
    {
        var order = new List<string>();

        Task<Result<Func<int, int>, DomainError>> funcTask =
            RecordAndReturn(order, "func", new Success<Func<int, int>, DomainError>(x => x * 2));

        Result<int, DomainError> applied = await funcTask.ApplyAsync(() =>
        {
            order.Add("arg-factory-invoked");
            return RecordAndReturn(order, "arg", new Success<int, DomainError>(21));
        });

        Assert.That(applied.Match(v => v, _ => -1), Is.EqualTo(42));
        Assert.That(order, Is.EqualTo(new[] { "func", "arg-factory-invoked", "arg" }));
    }

    [Test]
    public async Task ApplyAsync_FuncTaskFailure_ArgFactoryIsNeverInvoked()
    {
        Task<Result<Func<int, int>, DomainError>> funcTask =
            Task.FromResult<Result<Func<int, int>, DomainError>>(new Failure<Func<int, int>, DomainError>(DomainError.Invalid));
        var argFactoryWasCalled = false;

        Result<int, DomainError> applied = await funcTask.ApplyAsync(() =>
        {
            argFactoryWasCalled = true;
            return Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(21));
        });

        Assert.That(argFactoryWasCalled, Is.False);
        Assert.That(applied.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }

    [Test]
    public async Task ApplyAsync_ChainsTwoAsyncArguments_MirrorsRealAsyncConstructionFlow()
    {
        Task<Result<Func<int, Func<int, int>>, DomainError>> curriedTask =
            Task.FromResult(Func<Func<int, Func<int, int>>, DomainError>(a => b => a + b));

        Result<int, DomainError> sum = await curriedTask
            .ApplyAsync(() => Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(1)))
            .ApplyAsync(() => Task.FromResult<Result<int, DomainError>>(new Success<int, DomainError>(2)));

        Assert.That(sum.Match(v => v, _ => -1), Is.EqualTo(3));
    }

    private static async Task<Result<T, DomainError>> RecordAndReturn<T>(List<string> order, string label, Result<T, DomainError> value)
    {
        await Task.Yield();
        order.Add(label);
        return value;
    }
}
