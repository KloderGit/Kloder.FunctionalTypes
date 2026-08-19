namespace FunctionalTypes.TypedResult;

public static class ApplicativeExtensions
{
    public static Result<TFunc> Func<TFunc>(TFunc func) where TFunc : Delegate => new Success<TFunc>(func);

    public static Result<TResult> Apply<T, TResult>(this Result<Func<T, TResult>> func, Result<T> argResult) =>
        func.Bind(f => argResult.Map(f));
}
