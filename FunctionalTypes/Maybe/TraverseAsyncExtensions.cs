namespace FunctionalTypes.Maybe;

public static class TraverseAsyncExtensions
{
    // Awaits selector for each item in order; short-circuits to None (without awaiting the
    // rest) on the first None, otherwise collects all values into one Some.
    public static async Task<Maybe<IEnumerable<TR>>> TraverseAsync<T, TR>(this IEnumerable<T> source,
        Func<T, Task<Maybe<TR>>> selector)
    {
        var results = new List<TR>();
        foreach (var item in source)
        {
            var (hasValue, value) = await selector(item);
            if (!hasValue)
                return Maybe<IEnumerable<TR>>.None();
            results.Add(value!);
        }
        return Maybe<IEnumerable<TR>>.Some(results);
    }

    // Items are already-started tasks; awaited one by one, short-circuiting the same way.
    public static Task<Maybe<IEnumerable<T>>> SequenceAsync<T>(this IEnumerable<Task<Maybe<T>>> source) =>
        source.TraverseAsync(x => x);
}
