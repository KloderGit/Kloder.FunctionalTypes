using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeCheckAsync_SyncMaybeToTaskTests
{
    [Test]
    public async Task CheckAsync_OnJust_PredicateTrue_ReturnsUnchanged()
    {
        Maybe<int> maybe = Maybe<int>.Just(42);

        Maybe<int> checked_ = await maybe.CheckAsync(v => Task.FromResult(v > 0));

        Assert.That(checked_, Is.SameAs(maybe));
    }

    [Test]
    public async Task CheckAsync_OnJust_PredicateFalse_BecomesNothing()
    {
        Maybe<int> maybe = Maybe<int>.Just(-1);

        Maybe<int> checked_ = await maybe.CheckAsync(v => Task.FromResult(v > 0));

        Assert.That(checked_.IsNothing, Is.True);
    }

    [Test]
    public async Task CheckAsync_OnNothing_SkipsPredicateAndStaysNothing()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();
        var predicateWasCalled = false;

        Maybe<int> checked_ = await maybe.CheckAsync(v =>
        {
            predicateWasCalled = true;
            return Task.FromResult(v > 0);
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeCheckAsync_TaskToSyncPredicateTests
{
    [Test]
    public async Task Check_OnJust_PredicateFalse_BecomesNothing()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Just(-1));

        Maybe<int> checked_ = await maybeTask.Check(v => v > 0);

        Assert.That(checked_.IsNothing, Is.True);
    }

    [Test]
    public async Task Check_OnNothing_SkipsPredicate()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Nothing());
        var predicateWasCalled = false;

        Maybe<int> checked_ = await maybeTask.Check(v =>
        {
            predicateWasCalled = true;
            return v > 0;
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeCheckAsync_TaskToTaskTests
{
    [Test]
    public async Task CheckAsync_OnJust_PredicateFalse_BecomesNothing()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Just(-1));

        Maybe<int> checked_ = await maybeTask.CheckAsync(v => Task.FromResult(v > 0));

        Assert.That(checked_.IsNothing, Is.True);
    }

    [Test]
    public async Task CheckAsync_OnNothing_SkipsPredicate()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Nothing());
        var predicateWasCalled = false;

        Maybe<int> checked_ = await maybeTask.CheckAsync(v =>
        {
            predicateWasCalled = true;
            return Task.FromResult(v > 0);
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.IsNothing, Is.True);
    }
}
