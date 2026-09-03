namespace FunctionalTypes.TypedErrorResult;

public static class TraverseExtensions
{
    // Applies selector to each item; short-circuits with the first Failure, otherwise
    // collects all values into one Success.
    public static Result<IEnumerable<TR>, TError> Traverse<T, TR, TError>(this IEnumerable<T> source,
        Func<T, Result<TR, TError>> selector)
    {
        var results = new List<TR>();
        foreach (var item in source)
        {
            var (isSuccess, error, value) = selector(item);
            if (!isSuccess)
                return new Failure<IEnumerable<TR>, TError>(error!);
            results.Add(value!);
        }
        return new Success<IEnumerable<TR>, TError>(results);
    }

    public static Result<IEnumerable<T>, TError> Sequence<T, TError>(this IEnumerable<Result<T, TError>> source) =>
        source.Traverse(x => x);
}
