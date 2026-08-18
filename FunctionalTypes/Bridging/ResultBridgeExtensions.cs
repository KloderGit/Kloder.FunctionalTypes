using FunctionalTypes.TypedErrorResult;

namespace FunctionalTypes.Bridging;

public static class ResultBridgeExtensions
{
    // Simple Result to Typed Error Result
    public static Result<TR, TError> Map<TR, TError>(
        this SimpleResult.Result result,
        Func<TR> selector,
        Func<string, TError> errorSelector)
        => result.Match<Result<TR, TError>>(
            success: () => new Success<TR, TError>(selector()),
            failure: error => new Failure<TR, TError>(errorSelector(error)));

    public static Result<TR, TError> Bind<TR, TError>(
        this SimpleResult.Result result,
        Func<Result<TR, TError>> binder,
        Func<string, TError> errorSelector)
        => result.Match<Result<TR, TError>>(
            success: binder,
            failure: error => new Failure<TR, TError>(errorSelector(error)));

    
    // TypedResult to Typed Error Result
    public static Result<TR, TError> Map<T, TR, TError>(
        this TypedResult.Result<T> result,
        Func<T, TR> selector,
        Func<string, TError> errorSelector)
        => result.Match<Result<TR, TError>>(
            success: value => new Success<TR, TError>(selector(value)),
            failure: error => new Failure<TR, TError>(errorSelector(error)));

    public static Result<TR, TError> Bind<T, TR, TError>(
        this TypedResult.Result<T> result,
        Func<T, Result<TR, TError>> binder,
        Func<string, TError> errorSelector)
        => result.Match<Result<TR, TError>>(
            success: binder,
            failure: error => new Failure<TR, TError>(errorSelector(error)));

    // Typed Error Result to SimpleResult
    public static SimpleResult.Result Bind<T, TError>(
        this Result<T, TError> result,
        Func<T, SimpleResult.Result> binder,
        Func<TError, string> errorSelector)
        => result.Match<SimpleResult.Result>(
            success: binder,
            failure: error => new SimpleResult.Failure(errorSelector(error)));

    // Typed Error Result to TypedResult
    public static TypedResult.Result<TR> Map<T, TR, TError>(
        this Result<T, TError> result,
        Func<T, TR> selector,
        Func<TError, string> errorSelector)
        => result.Match<TypedResult.Result<TR>>(
            success: value => new TypedResult.Success<TR>(selector(value)),
            failure: error => new TypedResult.Failure<TR>(errorSelector(error)));

    public static TypedResult.Result<TR> Bind<T, TR, TError>(
        this Result<T, TError> result,
        Func<T, TypedResult.Result<TR>> binder,
        Func<TError, string> errorSelector)
        => result.Match<TypedResult.Result<TR>>(
            success: binder,
            failure: error => new TypedResult.Failure<TR>(errorSelector(error)));
}
