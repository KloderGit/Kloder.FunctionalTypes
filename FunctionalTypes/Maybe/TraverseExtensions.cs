namespace FunctionalTypes.Maybe;

public static class TraverseExtensions
{
    // Applies selector to each item; short-circuits to None on the first None, otherwise
    // collects all values into one Some.
    public static Maybe<IEnumerable<TR>> Traverse<T, TR>(this IEnumerable<T> source, Func<T, Maybe<TR>> selector)
    {
        var results = new List<TR>();
        foreach (var item in source)
        {
            var (hasValue, value) = selector(item);
            if (!hasValue)
                return Maybe<IEnumerable<TR>>.None();
            results.Add(value!);
        }
        return Maybe<IEnumerable<TR>>.Some(results);
    }

    public static Maybe<IEnumerable<T>> Sequence<T>(this IEnumerable<Maybe<T>> source) =>
        source.Traverse(x => x);
}
