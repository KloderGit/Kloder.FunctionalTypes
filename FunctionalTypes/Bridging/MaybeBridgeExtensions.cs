using FunctionalTypes.Maybe;

namespace FunctionalTypes.Bridging;

public static class MaybeBridgeExtensions
{
    // Maybe to TypedResult
    public static TypedResult.Result<T> ToResult<T>(this Maybe<T> maybe, string nothingMessage) =>
        maybe.Match<TypedResult.Result<T>>(
            just: value => new TypedResult.Success<T>(value),
            nothing: () => new TypedResult.Failure<T>(nothingMessage));

    // Maybe to TypedErrorResult
    public static TypedErrorResult.Result<T, TError> ToResult<T, TError>(this Maybe<T> maybe, Func<TError> errorFactory) =>
        maybe.Match<TypedErrorResult.Result<T, TError>>(
            just: value => new TypedErrorResult.Success<T, TError>(value),
            nothing: () => new TypedErrorResult.Failure<T, TError>(errorFactory()));

    // TypedResult to Maybe (error message is discarded — Maybe carries no payload for it)
    public static Maybe<T> ToMaybe<T>(this TypedResult.Result<T> result) =>
        result.Match<Maybe<T>>(
            success: value => Maybe<T>.Just(value),
            failure: _ => Maybe<T>.Nothing());

    // TypedErrorResult to Maybe (error value is discarded)
    public static Maybe<T> ToMaybe<T, TError>(this TypedErrorResult.Result<T, TError> result) =>
        result.Match<Maybe<T>>(
            success: value => Maybe<T>.Just(value),
            failure: _ => Maybe<T>.Nothing());
}
