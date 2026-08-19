using FunctionalTypes.TypedErrorResult;
using FunctionalTypes.TypedResult;

namespace FunctionalTypes.SimpleResult;

public static class ApplicativeExtensions
{
    public static Result<TFunc> Func<TFunc>(this Result result, TFunc func) where TFunc : Delegate
        => result.Match<Result<TFunc>>(
            success: () => new Success<TFunc>(func),
            failure: s => new Failure<TFunc>(s));

    public static Result<TFunc, TError> Func<TFunc, TError>(this Result result, TFunc func,
        Func<string, TError> errorSelector) where TFunc : Delegate
        => result.Match<Result<TFunc, TError>>(
            success: () => new Success<TFunc, TError>(func),
            failure: s => new Failure<TFunc, TError>(errorSelector(s)));
}
