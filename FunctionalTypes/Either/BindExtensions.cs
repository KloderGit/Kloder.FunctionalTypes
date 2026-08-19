using FunctionalTypes.TypedResult;

namespace FunctionalTypes.Either;

public static class BindExtensions
{
    // Each branch continues into its own next Result step
    public static Result<TR> Bind<TLeft, TRight, TR>(
        this Result<Either<TLeft, TRight>> result,
        Func<TLeft, Result<TR>> onLeft,
        Func<TRight, Result<TR>> onRight) =>
        result.Bind(either => either.Match(onLeft, onRight));

    // Both branches carry the same type — collapse to it and continue with the regular Result.Bind chain
    public static Result<T> Collapse<T>(this Result<Either<T, T>> result) =>
        result.Map(either => either.Match(x => x, x => x));
}
