using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeReferenceExtensionsTests
{
    [Test]
    public void ToMaybe_NonNullReference_ProducesSome()
    {
        string? value = "hello";

        Maybe<string> maybe = value.ToMaybe();

        Assert.That(maybe.Match(v => v, () => "none"), Is.EqualTo("hello"));
    }

    [Test]
    public void ToMaybe_NullReference_ProducesNone()
    {
        string? value = null;

        Maybe<string> maybe = value.ToMaybe();

        Assert.That(maybe.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeValueExtensionsTests
{
    [Test]
    public void ToMaybe_PlainStruct_ProducesSome()
    {
        var value = 42;

        Maybe<int> maybe = value.ToMaybe();

        Assert.That(maybe.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public void ToMaybe_NullableStructWithValue_ProducesSome()
    {
        int? value = 42;

        Maybe<int> maybe = value.ToMaybe();

        Assert.That(maybe.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public void ToMaybe_NullableStructWithoutValue_ProducesNone()
    {
        int? value = null;

        Maybe<int> maybe = value.ToMaybe();

        Assert.That(maybe.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeSequenceExtensionsTests
{
    [Test]
    public void ToMaybe_NullSequence_ProducesNone()
    {
        IEnumerable<int>? value = null;

        Maybe<IReadOnlyCollection<int>> maybe = value.ToMaybe();

        Assert.That(maybe.IsNone, Is.True);
    }

    [Test]
    public void ToMaybe_EmptyList_ProducesNone()
    {
        // Declared as IEnumerable<T>, not List<T> — see the overload-resolution note on
        // MaybeSequenceExtensions.ToMaybe: a variable statically typed as the concrete
        // collection class resolves to MaybeReferenceExtensions.ToMaybe instead (null-check
        // only, no empty-check), because that overload is an identity match for the argument.
        IEnumerable<int> value = new List<int>();

        Maybe<IReadOnlyCollection<int>> maybe = value.ToMaybe();

        Assert.That(maybe.IsNone, Is.True);
    }

    [Test]
    public void ToMaybe_EmptyLazySequence_ProducesNone()
    {
        IEnumerable<int> value = Enumerable.Range(0, 0);

        Maybe<IReadOnlyCollection<int>> maybe = value.ToMaybe();

        Assert.That(maybe.IsNone, Is.True);
    }

    [Test]
    public void ToMaybe_NonEmptyList_ProducesSomeWithSameItems()
    {
        // Declared as IEnumerable<T> — see note on ToMaybe_EmptyList_ProducesNone above.
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
        // Some(emptyList), not None. null still correctly becomes None either way. This is the
        // documented, accepted limitation — see MaybeSequenceExtensions.ToMaybe's XML doc.
        var value = new List<int>();

        Maybe<List<int>> maybe = value.ToMaybe();

        Assert.That(maybe.IsSome, Is.True);
        Assert.That(maybe.Match(v => v.Count, () => -1), Is.EqualTo(0));
    }

    [Test]
    public void ToMaybe_NonEmptyLazySequence_MaterializesOnceAndProducesSome()
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
