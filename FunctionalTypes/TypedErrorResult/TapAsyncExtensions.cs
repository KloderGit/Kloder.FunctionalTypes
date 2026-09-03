namespace FunctionalTypes.TypedErrorResult;

public static class TapAsyncExtensions
{
    // Tap → Task
    public static Task<Result<T, TError>> TapAsync<T, TError>(this Result<T, TError> result, Func<T, Task> action) =>
        result.Match<Task<Result<T, TError>>>(
            success: async value => { await action(value); return result; },
            failure: _ => Task.FromResult(result)
        );

    public static Task<Result<T, TError>> TapAsync<T, TError>(this Result<T, TError> result, Func<Task> action) =>
        result.Match<Task<Result<T, TError>>>(
            success: async _ => { await action(); return result; },
            failure: _ => Task.FromResult(result)
        );

    public static Task<Result<T, TError>> TapErrorAsync<T, TError>(this Result<T, TError> result, Func<TError, Task> action) =>
        result.Match<Task<Result<T, TError>>>(
            success: _ => Task.FromResult(result),
            failure: async error => { await action(error); return result; }
        );

    public static Task<Result<T, TError>> TapErrorAsync<T, TError>(this Result<T, TError> result, Func<Task> action) =>
        result.Match<Task<Result<T, TError>>>(
            success: _ => Task.FromResult(result),
            failure: async _ => { await action(); return result; }
        );


    // Task → Tap
    public static async Task<Result<T, TError>> Tap<T, TError>(this Task<Result<T, TError>> resultTask, Action<T> action) =>
        (await resultTask).Tap(action);

    public static async Task<Result<T, TError>> Tap<T, TError>(this Task<Result<T, TError>> resultTask, Action action) =>
        (await resultTask).Tap(action);

    public static async Task<Result<T, TError>> TapError<T, TError>(this Task<Result<T, TError>> resultTask, Action<TError> action) =>
        (await resultTask).TapError(action);

    public static async Task<Result<T, TError>> TapError<T, TError>(this Task<Result<T, TError>> resultTask, Action action) =>
        (await resultTask).TapError(action);


    // Task → Task
    public static async Task<Result<T, TError>> TapAsync<T, TError>(this Task<Result<T, TError>> resultTask, Func<T, Task> action) =>
        await (await resultTask).TapAsync(action);

    public static async Task<Result<T, TError>> TapAsync<T, TError>(this Task<Result<T, TError>> resultTask, Func<Task> action) =>
        await (await resultTask).TapAsync(action);

    public static async Task<Result<T, TError>> TapErrorAsync<T, TError>(this Task<Result<T, TError>> resultTask, Func<TError, Task> action) =>
        await (await resultTask).TapErrorAsync(action);

    public static async Task<Result<T, TError>> TapErrorAsync<T, TError>(this Task<Result<T, TError>> resultTask, Func<Task> action) =>
        await (await resultTask).TapErrorAsync(action);
}
