namespace FunctionalTypes.Maybe;

public static class MaybeSequenceExtensions
{
    /// <summary>
    /// Converts a sequence to <see cref="Maybe{T}"/>: <c>null</c> or empty (after materializing)
    /// becomes <c>Nothing</c>, otherwise <c>Just</c> of the materialized collection.
    /// </summary>
    /// <remarks>
    /// Only resolves when the argument's <em>static</em> type is <see cref="IEnumerable{T}"/> itself
    /// (an interface-typed variable, or a method whose return type is declared as
    /// <see cref="IEnumerable{T}"/>). When the static type is a concrete reference type instead —
    /// <c>List&lt;T&gt;</c>, <c>T[]</c>, etc. — C# overload resolution picks
    /// <see cref="MaybeReferenceExtensions.ToMaybe{T}"/> instead, because that overload is an
    /// identity match for the argument while this one requires an interface conversion, and identity
    /// always wins. That overload only null-checks — it does not apply the "empty means Nothing" rule.
    /// If you need the empty-check for a concretely-typed collection variable, upcast it to
    /// <see cref="IEnumerable{T}"/> first (or declare the variable as <see cref="IEnumerable{T}"/>).
    /// </remarks>
    public static Maybe<IReadOnlyCollection<T>> ToMaybe<T>(this IEnumerable<T>? value)
    {
        if (value is null)
            return Maybe<IReadOnlyCollection<T>>.Nothing();

        var materialized = value as IReadOnlyCollection<T> ?? value.ToList();
        return materialized.Count == 0
            ? Maybe<IReadOnlyCollection<T>>.Nothing()
            : Maybe<IReadOnlyCollection<T>>.Just(materialized);
    }
}
