using FunctionalTypes.SimpleResult;

namespace FunctionalTypes.TypedResult;

public static class TraverseExtensions
{
    // Applies selector to each item; short-circuits with the first Failure, otherwise
    // collects all values into one Success.
    public static Result<IEnumerable<TR>> Traverse<T, TR>(this IEnumerable<T> source, Func<T, Result<TR>> selector)
    {
        var results = new List<TR>();
        foreach (var item in source)
        {
            var (isSuccess, error, value) = selector(item);
            if (!isSuccess)
                return new Failure<IEnumerable<TR>>(error!);
            results.Add(value!);
        }
        return new Success<IEnumerable<TR>>(results);
    }
    
    public static Result Traverse<T>(this IEnumerable<T> source, Func<T, Result> selector)
    {
        foreach (var item in source)
        {
            var (isSuccess, error) = selector(item);
            if (!isSuccess)
                return new Failure(error!);
        }
        return new Success();
    }

    public static Result<IEnumerable<T>> Sequence<T>(this IEnumerable<Result<T>> source) =>
        source.Traverse(x => x);
}
