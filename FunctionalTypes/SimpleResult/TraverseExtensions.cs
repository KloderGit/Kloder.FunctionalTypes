namespace FunctionalTypes.SimpleResult;

public static class TraverseExtensions
{
    public static Result Traverse<T, TR>(this IEnumerable<T> source, Func<T, Result> selector)
    {
        foreach (var item in source)
        {
            var (isSuccess, error) = selector(item);
            if (!isSuccess)
                return new Failure(error!);
        }
        return new Success();
    }
    
    public static Result Sequence<T>(this IEnumerable<Result> source) =>
        source.Traverse<Result, Result>(x => x);
}