using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeCheckAsync_SyncMaybeToTaskTests
{
    [Test]
    public async Task CheckAsync_OnSome_PredicateTrue_ReturnsUnchanged()
    {
        Maybe<int> maybe = Maybe<int>.Some(42);

        Maybe<int> checked_ = await maybe.CheckAsync(v => Task.FromResult(v > 0));

        Assert.That(checked_, Is.SameAs(maybe));
    }

    [Test]
    public async Task CheckAsync_OnSome_PredicateFalse_BecomesNone()
    {
        Maybe<int> maybe = Maybe<int>.Some(-1);

        Maybe<int> checked_ = await maybe.CheckAsync(v => Task.FromResult(v > 0));

        Assert.That(checked_.IsNone, Is.True);
    }

    [Test]
    public async Task CheckAsync_OnNone_SkipsPredicateAndStaysNone()
    {
        Maybe<int> maybe = Maybe<int>.None();
        var predicateWasCalled = false;

        Maybe<int> checked_ = await maybe.CheckAsync(v =>
        {
            predicateWasCalled = true;
            return Task.FromResult(v > 0);
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeCheckAsync_TaskToSyncPredicateTests
{
    [Test]
    public async Task Check_OnSome_PredicateFalse_BecomesNone()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Some(-1));

        Maybe<int> checked_ = await maybeTask.Check(v => v > 0);

        Assert.That(checked_.IsNone, Is.True);
    }

    [Test]
    public async Task Check_OnNone_SkipsPredicate()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.None());
        var predicateWasCalled = false;

        Maybe<int> checked_ = await maybeTask.Check(v =>
        {
            predicateWasCalled = true;
            return v > 0;
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeCheckAsync_TaskToTaskTests
{
    [Test]
    public async Task CheckAsync_OnSome_PredicateFalse_BecomesNone()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Some(-1));

        Maybe<int> checked_ = await maybeTask.CheckAsync(v => Task.FromResult(v > 0));

        Assert.That(checked_.IsNone, Is.True);
    }

    [Test]
    public async Task CheckAsync_OnNone_SkipsPredicate()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.None());
        var predicateWasCalled = false;

        Maybe<int> checked_ = await maybeTask.CheckAsync(v =>
        {
            predicateWasCalled = true;
            return Task.FromResult(v > 0);
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.IsNone, Is.True);
    }
}
