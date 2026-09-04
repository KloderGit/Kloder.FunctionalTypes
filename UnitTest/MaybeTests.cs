using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeFactoryTests
{
    [Test]
    public void Just_HasValue_IsJustTrue_IsNothingFalse()
    {
        Maybe<int> maybe = Maybe<int>.Just(42);

        Assert.That(maybe.IsJust, Is.True);
        Assert.That(maybe.IsNothing, Is.False);
    }

    [Test]
    public void Nothing_IsJustFalse_IsNothingTrue()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();

        Assert.That(maybe.IsJust, Is.False);
        Assert.That(maybe.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeMapTests
{
    [Test]
    public void Map_OnJust_TransformsValue()
    {
        Maybe<int> maybe = Maybe<int>.Just(21);

        Maybe<int> mapped = maybe.Map(v => v * 2);

        Assert.That(mapped.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public void Map_OnNothing_SkipsSelectorAndStaysNothing()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();
        var selectorWasCalled = false;

        Maybe<int> mapped = maybe.Map(v =>
        {
            selectorWasCalled = true;
            return v * 2;
        });

        Assert.That(selectorWasCalled, Is.False);
        Assert.That(mapped.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeBindTests
{
    [Test]
    public void Bind_OnJust_RunsBinder()
    {
        Maybe<int> maybe = Maybe<int>.Just(21);

        Maybe<string> bound = maybe.Bind(v => Maybe<string>.Just((v * 2).ToString()));

        Assert.That(bound.Match(v => v, () => "nothing"), Is.EqualTo("42"));
    }

    [Test]
    public void Bind_OnNothing_SkipsBinderAndStaysNothing()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();
        var binderWasCalled = false;

        Maybe<string> bound = maybe.Bind(v =>
        {
            binderWasCalled = true;
            return Maybe<string>.Just(v.ToString());
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeMatchTests
{
    [Test]
    public void Match_OnJust_RunsJustBranch()
    {
        Maybe<int> maybe = Maybe<int>.Just(42);

        string outcome = maybe.Match(v => v.ToString(), () => "nothing");

        Assert.That(outcome, Is.EqualTo("42"));
    }

    [Test]
    public void Match_OnNothing_RunsNothingBranch()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();

        string outcome = maybe.Match(v => v.ToString(), () => "nothing");

        Assert.That(outcome, Is.EqualTo("nothing"));
    }
}

[TestFixture]
public class MaybeTapTests
{
    [Test]
    public void Tap_OnJust_RunsActionAndReturnsUnchanged()
    {
        Maybe<int> maybe = Maybe<int>.Just(42);
        var seen = -1;

        Maybe<int> tapped = maybe.Tap(v => seen = v);

        Assert.That(seen, Is.EqualTo(42));
        Assert.That(tapped, Is.SameAs(maybe));
    }

    [Test]
    public void Tap_OnNothing_SkipsAction()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();
        var tapWasCalled = false;

        Maybe<int> tapped = maybe.Tap(_ => tapWasCalled = true);

        Assert.That(tapWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(maybe));
    }

    [Test]
    public void TapNothing_OnNothing_RunsAction()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();
        var tapNothingWasCalled = false;

        Maybe<int> tapped = maybe.TapNothing(() => tapNothingWasCalled = true);

        Assert.That(tapNothingWasCalled, Is.True);
        Assert.That(tapped, Is.SameAs(maybe));
    }

    [Test]
    public void TapNothing_OnJust_SkipsAction()
    {
        Maybe<int> maybe = Maybe<int>.Just(42);
        var tapNothingWasCalled = false;

        Maybe<int> tapped = maybe.TapNothing(() => tapNothingWasCalled = true);

        Assert.That(tapNothingWasCalled, Is.False);
        Assert.That(tapped, Is.SameAs(maybe));
    }
}

[TestFixture]
public class MaybeCheckTests
{
    [Test]
    public void Check_OnJust_PredicateTrue_ReturnsUnchanged()
    {
        Maybe<int> maybe = Maybe<int>.Just(42);

        Maybe<int> checked_ = maybe.Check(v => v > 0);

        Assert.That(checked_, Is.SameAs(maybe));
    }

    [Test]
    public void Check_OnJust_PredicateFalse_BecomesNothing()
    {
        Maybe<int> maybe = Maybe<int>.Just(-1);

        Maybe<int> checked_ = maybe.Check(v => v > 0);

        Assert.That(checked_.IsNothing, Is.True);
    }

    [Test]
    public void Check_OnNothing_SkipsPredicateAndStaysNothing()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();
        var predicateWasCalled = false;

        Maybe<int> checked_ = maybe.Check(v =>
        {
            predicateWasCalled = true;
            return v > 0;
        });

        Assert.That(predicateWasCalled, Is.False);
        Assert.That(checked_.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeGetValueOrDefaultTests
{
    [Test]
    public void GetValueOrDefault_OnJust_ReturnsValue()
    {
        Maybe<int> maybe = Maybe<int>.Just(42);

        Assert.That(maybe.GetValueOrDefault(-1), Is.EqualTo(42));
    }

    [Test]
    public void GetValueOrDefault_OnNothing_ReturnsDefault()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();

        Assert.That(maybe.GetValueOrDefault(-1), Is.EqualTo(-1));
    }
}

[TestFixture]
public class MaybeDeconstructTests
{
    [Test]
    public void Deconstruct_OnJust_YieldsHasValueTrueAndValue()
    {
        Maybe<int> maybe = Maybe<int>.Just(42);

        var (hasValue, value) = maybe;

        Assert.That(hasValue, Is.True);
        Assert.That(value, Is.EqualTo(42));
    }

    [Test]
    public void Deconstruct_OnNothing_YieldsHasValueFalseAndDefault()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();

        var (hasValue, value) = maybe;

        Assert.That(hasValue, Is.False);
        Assert.That(value, Is.EqualTo(0));
    }
}
