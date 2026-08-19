namespace FunctionalTypes.TypedErrorResult;

public static class ApplicativeExtensions
{
    public static Result<TFunc, TError> Func<TFunc, TError>(TFunc func) where TFunc : Delegate => new Success<TFunc, TError>(func);

    public static Result<TResult, TError> Apply<T, TResult, TError>(this Result<Func<T, TResult>, TError> func, Result<T, TError> argResult) =>
        func.Bind(f => argResult.Map(f));
}
