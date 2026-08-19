namespace FunctionalTypes.TypedResult;

public static class ApplicativeExtensions
{
    public static Result<TFunc> Func<TFunc>(TFunc func) where TFunc : Delegate => new Success<TFunc>(func);

    public static Result<TResult> Apply<T, TResult>(this Result<Func<T, TResult>> func, Result<T> argResult) =>
        func.Bind(f => argResult.Map(f));

    // func → sync, arg → async (lazy: only produced/awaited when func succeeded)
    public static Task<Result<TResult>> ApplyAsync<T, TResult>(this Result<Func<T, TResult>> func,
        Func<Task<Result<T>>> argResultTaskFactory) =>
        func.BindAsync(async f => (await argResultTaskFactory()).Map(f));

    // func → async, arg → sync
    public static async Task<Result<TResult>> Apply<T, TResult>(this Task<Result<Func<T, TResult>>> funcTask,
        Result<T> argResult) =>
        (await funcTask).Apply(argResult);

    // func → async, arg → async (sequential: func is awaited first; arg is only produced/awaited on success)
    public static async Task<Result<TResult>> ApplyAsync<T, TResult>(this Task<Result<Func<T, TResult>>> funcTask,
        Func<Task<Result<T>>> argResultTaskFactory) =>
        await (await funcTask).ApplyAsync(argResultTaskFactory);
}
