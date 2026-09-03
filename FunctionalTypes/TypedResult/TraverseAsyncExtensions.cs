namespace FunctionalTypes.TypedResult;

public static class TraverseAsyncExtensions
{
    // Awaits selector for each item in order; short-circuits (without awaiting the rest)
    // on the first Failure, otherwise collects all values into one Success.
    public static async Task<Result<IEnumerable<TR>>> TraverseAsync<T, TR>(this IEnumerable<T> source,
        Func<T, Task<Result<TR>>> selector)
    {
        var results = new List<TR>();
        foreach (var item in source)
        {
            var (isSuccess, error, value) = await selector(item);
            if (!isSuccess)
                return new Failure<IEnumerable<TR>>(error!);
            results.Add(value!);
        }
        return new Success<IEnumerable<TR>>(results);
    }

    // Items are already-started tasks; awaited one by one, short-circuiting the same way.
    public static Task<Result<IEnumerable<T>>> SequenceAsync<T>(this IEnumerable<Task<Result<T>>> source) =>
        source.TraverseAsync(x => x);
}
