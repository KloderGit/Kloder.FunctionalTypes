namespace FunctionalTypes.TypedErrorResult;

public static class ApplicativeExtensions
{
    public static Result<TFunc, TError> Func<TFunc, TError>(TFunc func) where TFunc : Delegate => new Success<TFunc, TError>(func);

    public static Result<TResult, TError> Apply<T, TResult, TError>(this Result<Func<T, TResult>, TError> func, Result<T, TError> argResult) =>
        func.Bind(f => argResult.Map(f));

    // func → sync, arg → async (lazy: only produced/awaited when func succeeded)
    public static Task<Result<TResult, TError>> ApplyAsync<T, TResult, TError>(this Result<Func<T, TResult>, TError> func,
        Func<Task<Result<T, TError>>> argResultTaskFactory) =>
        func.BindAsync(async f => (await argResultTaskFactory()).Map(f));

    // func → async, arg → sync
    public static async Task<Result<TResult, TError>> Apply<T, TResult, TError>(this Task<Result<Func<T, TResult>, TError>> funcTask,
        Result<T, TError> argResult) =>
        (await funcTask).Apply(argResult);

    // func → async, arg → async (sequential: func is awaited first; arg is only produced/awaited on success)
    public static async Task<Result<TResult, TError>> ApplyAsync<T, TResult, TError>(this Task<Result<Func<T, TResult>, TError>> funcTask,
        Func<Task<Result<T, TError>>> argResultTaskFactory) =>
        await (await funcTask).ApplyAsync(argResultTaskFactory);
}
