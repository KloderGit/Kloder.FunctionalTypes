using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeReferenceExtensionsTests
{
    [Test]
    public void ToMaybe_NonNullReference_ProducesJust()
    {
        string? value = "hello";

        Maybe<string> maybe = value.ToMaybe();

        Assert.That(maybe.Match(v => v, () => "nothing"), Is.EqualTo("hello"));
    }

    [Test]
    public void ToMaybe_NullReference_ProducesNothing()
    {
        string? value = null;

        Maybe<string> maybe = value.ToMaybe();

        Assert.That(maybe.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeValueExtensionsTests
{
    [Test]
    public void ToMaybe_PlainStruct_ProducesJust()
    {
        var value = 42;

        Maybe<int> maybe = value.ToMaybe();

        Assert.That(maybe.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public void ToMaybe_NullableStructWithValue_ProducesJust()
    {
        int? value = 42;

        Maybe<int> maybe = value.ToMaybe();

        Assert.That(maybe.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public void ToMaybe_NullableStructWithoutValue_ProducesNothing()
    {
        int? value = null;

        Maybe<int> maybe = value.ToMaybe();

        Assert.That(maybe.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeSequenceExtensionsTests
{
    [Test]
    public void ToMaybe_NullSequence_ProducesNothing()
    {
        IEnumerable<int>? value = null;

        Maybe<IReadOnlyCollection<int>> maybe = value.ToMaybe();

        Assert.That(maybe.IsNothing, Is.True);
    }

    [Test]
    public void ToMaybe_EmptyList_ProducesNothing()
    {
        // Declared as IEnumerable<T>, not List<T> — see the overload-resolution note on
        // MaybeSequenceExtensions.ToMaybe: a variable statically typed as the concrete
        // collection class resolves to MaybeReferenceExtensions.ToMaybe instead (null-check
        // only, no empty-check), because that overload is an identity match for the argument.
        IEnumerable<int> value = new List<int>();

        Maybe<IReadOnlyCollection<int>> maybe = value.ToMaybe();

        Assert.That(maybe.IsNothing, Is.True);
    }

    [Test]
    public void ToMaybe_EmptyLazySequence_ProducesNothing()
    {
        IEnumerable<int> value = Enumerable.Range(0, 0);

        Maybe<IReadOnlyCollection<int>> maybe = value.ToMaybe();

        Assert.That(maybe.IsNothing, Is.True);
    }

    [Test]
    public void ToMaybe_NonEmptyList_ProducesJustWithSameItems()
    {
        // Declared as IEnumerable<T> — see note on ToMaybe_EmptyList_ProducesNothing above.
        IEnumerable<int> value = new List<int> { 1, 2, 3 };

        Maybe<IReadOnlyCollection<int>> maybe = value.ToMaybe();

        Assert.That(maybe.Match(v => v, () => []), Is.EqualTo(value));
    }

    [Test]
    public void ToMaybe_ConcreteListTypedVariable_SkipsEmptyCheck_KnownLimitation()
    {
        // When the variable is statically typed as the concrete List<T> (not IEnumerable<T>),
        // overload resolution picks MaybeReferenceExtensions.ToMaybe<T> where T : class instead
        // (identity match beats the IEnumerable<T> conversion match) — so an empty list becomes
        // Just(emptyList), not Nothing. null still correctly becomes Nothing either way. This is the
        // documented, accepted limitation — see MaybeSequenceExtensions.ToMaybe's XML doc.
        var value = new List<int>();

        Maybe<List<int>> maybe = value.ToMaybe();

        Assert.That(maybe.IsJust, Is.True);
        Assert.That(maybe.Match(v => v.Count, () => -1), Is.EqualTo(0));
    }

    [Test]
    public void ToMaybe_NonEmptyLazySequence_MaterializesOnceAndProducesJust()
    {
        var enumerationCount = 0;
        IEnumerable<int> LazySequence()
        {
            enumerationCount++;
            yield return 1;
            yield return 2;
        }

        Maybe<IReadOnlyCollection<int>> maybe = LazySequence().ToMaybe();

        Assert.That(enumerationCount, Is.EqualTo(1));
        Assert.That(maybe.Match(v => v.Count, () => -1), Is.EqualTo(2));
        // Reading the materialized value again must not re-enumerate the original generator.
        Assert.That(maybe.Match(v => v.Count, () => -1), Is.EqualTo(2));
        Assert.That(enumerationCount, Is.EqualTo(1));
    }
}
