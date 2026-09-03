using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeFactoryTests
{
    [Test]
    public void Some_HasValue_IsSomeTrue_IsNoneFalse()
    {
        Maybe<int> maybe = Maybe<int>.Some(42);

        Assert.That(maybe.IsSome, Is.True);
        Assert.That(maybe.IsNone, Is.False);
    }

    [Test]
    public void None_IsSomeFalse_IsNoneTrue()
    {
        Maybe<int> maybe = Maybe<int>.None();

        Assert.That(maybe.IsSome, Is.False);
        Assert.That(maybe.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeMapTests
{
    [Test]
    public void Map_OnSome_TransformsValue()
    {
        Maybe<int> maybe = Maybe<int>.Some(21);

        Maybe<int> mapped = maybe.Map(v => v * 2);

        Assert.That(mapped.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public void Map_OnNone_SkipsSelectorAndStaysNone()
    {
        Maybe<int> maybe = Maybe<int>.None();
        var selectorWasCalled = false;

        Maybe<int> mapped = maybe.Map(v =>
        {
            selectorWasCalled = true;
            return v * 2;
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeBindTests
{
    [Test]
    public void Bind_OnSome_RunsBinder()
    {
        Maybe<int> maybe = Maybe<int>.Some(21);

        Maybe<string> bound = maybe.Bind(v => Maybe<string>.Some((v * 2).ToString()));

        Assert.That(bound.Match(v => v, () => "none"), Is.EqualTo("42"));
    }

    [Test]
    public void Bind_OnNone_SkipsBinderAndStaysNone()
    {
        Maybe<int> maybe = Maybe<int>.None();
        var binderWasCalled = false;

        Maybe<string> bound = maybe.Bind(v =>
        {
            binderWasCalled = true;
            return Maybe<string>.Some(v.ToString());
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeMatchTests
{
    [Test]
    public void Match_OnSome_RunsSomeBranch()
    {
        Maybe<int> maybe = Maybe<int>.Some(42);

        string outcome = maybe.Match(v => v.ToString(), () => "none");

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public void Match_OnNone_RunsNoneBranch()
    {
        Maybe<int> maybe = Maybe<int>.None();

        string outcome = maybe.Match(v => v.ToString(), () => "none");

        Assert.That(outcome, Is.EqualTo("none"));
    }
}

[TestFixture]
public class MaybeTapTests
{
    [Test]
    public void Tap_OnSome_RunsActionAndReturnsUnchanged()
    {
        Maybe<int> maybe = Maybe<int>.Some(42);
        var seen = -1;

        Maybe<int> tapped = maybe.Tap(v => seen = v);

        Assert.That(seen, Is.EqualTo(42));
        Assert.That(tapped, Is.SameAs(maybe));
    }

    [Test]
    public void Tap_OnNone_SkipsAction()
    {
        Maybe<int> maybe = Maybe<int>.None();
        var tapWasCalled = false;

        Maybe<int> tapped = maybe.Tap(_ => tapWasCalled = true);

        Assert.That(tapWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(maybe));
    }

    [Test]
    public void TapNone_OnNone_RunsAction()
    {
        Maybe<int> maybe = Maybe<int>.None();
        var tapNoneWasCalled = false;

        Maybe<int> tapped = maybe.TapNone(() => tapNoneWasCalled = true);

        Assert.That(tapNoneWasCalled, Is.True);
        Assert.That(tapped, Is.SameAs(maybe));
    }

    [Test]
    public void TapNone_OnSome_SkipsAction()
    {
        Maybe<int> maybe = Maybe<int>.Some(42);
        var tapNoneWasCalled = false;

        Maybe<int> tapped = maybe.TapNone(() => tapNoneWasCalled = true);

        Assert.That(tapNoneWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(maybe));
    }
}

[TestFixture]
public class MaybeCheckTests
{
    [Test]
    public void Check_OnSome_PredicateTrue_ReturnsUnchanged()
    {
        Maybe<int> maybe = Maybe<int>.Some(42);

        Maybe<int> checked_ = maybe.Check(v => v > 0);

        Assert.That(checked_, Is.SameAs(maybe));
    }

    [Test]
    public void Check_OnSome_PredicateFalse_BecomesNone()
    {
        Maybe<int> maybe = Maybe<int>.Some(-1);

        Maybe<int> checked_ = maybe.Check(v => v > 0);

        Assert.That(checked_.IsNone, Is.True);
    }

    [Test]
    public void Check_OnNone_SkipsPredicateAndStaysNone()
    {
        Maybe<int> maybe = Maybe<int>.None();
        var predicateWasCalled = false;

        Maybe<int> checked_ = maybe.Check(v =>
        {
            predicateWasCalled = true;
            return v > 0;
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeGetValueOrDefaultTests
{
    [Test]
    public void GetValueOrDefault_OnSome_ReturnsValue()
    {
        Maybe<int> maybe = Maybe<int>.Some(42);

        Assert.That(maybe.GetValueOrDefault(-1), Is.EqualTo(42));
    }

    [Test]
    public void GetValueOrDefault_OnNone_ReturnsDefault()
    {
        Maybe<int> maybe = Maybe<int>.None();

        Assert.That(maybe.GetValueOrDefault(-1), Is.EqualTo(-1));
    }
}

[TestFixture]
public class MaybeDeconstructTests
{
    [Test]
    public void Deconstruct_OnSome_YieldsHasValueTrueAndValue()
    {
        Maybe<int> maybe = Maybe<int>.Some(42);

        var (hasValue, value) = maybe;

        Assert.That(hasValue, Is.True);
        Assert.That(value, Is.EqualTo(42));
    }

    [Test]
    public void Deconstruct_OnNone_YieldsHasValueFalseAndDefault()
    {
        Maybe<int> maybe = Maybe<int>.None();

        var (hasValue, value) = maybe;

        Assert.That(hasValue, Is.False);
        Assert.That(value, Is.EqualTo(0));
    }
}
