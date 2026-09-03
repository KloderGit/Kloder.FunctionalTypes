namespace FunctionalTypes.Maybe;

public static class ApplicativeExtensions
{
    public static Maybe<TFunc> Func<TFunc>(TFunc func) where TFunc : Delegate => new Some<TFunc>(func);

    public static Maybe<TResult> Apply<T, TResult>(this Maybe<Func<T, TResult>> func, Maybe<T> argResult) =>
        func.Bind(f => argResult.Map(f));

    // func → sync, arg → async (lazy: only produced/awaited when func succeeded)
    public static Task<Maybe<TResult>> ApplyAsync<T, TResult>(this Maybe<Func<T, TResult>> func,
        Func<Task<Maybe<T>>> argResultTaskFactory) =>
        func.BindAsync(async f => (await argResultTaskFactory()).Map(f));

    // func → async, arg → sync
    public static async Task<Maybe<TResult>> Apply<T, TResult>(this Task<Maybe<Func<T, TResult>>> funcTask,
        Maybe<T> argResult) =>
        (await funcTask).Apply(argResult);

    // func → async, arg → async (sequential: func is awaited first; arg is only produced/awaited on success)
    public static async Task<Maybe<TResult>> ApplyAsync<T, TResult>(this Task<Maybe<Func<T, TResult>>> funcTask,
        Func<Task<Maybe<T>>> argResultTaskFactory) =>
        await (await funcTask).ApplyAsync(argResultTaskFactory);
}
