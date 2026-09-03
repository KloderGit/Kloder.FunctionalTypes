using FunctionalTypes.SimpleResult;

namespace FunctionalTypes.TypedResult;

public static class TapAsyncExtensions
{
    // Tap → Task
    public static Task<Result<T>> TapAsync<T>(this Result<T> result, Func<T, Task> action) =>
        result.Match<Task<Result<T>>>(
            success: async value => { await action(value); return result; },
            failure: _ => Task.FromResult(result)
        );

    public static Task<Result<T>> TapErrorAsync<T>(this Result<T> result, Func<string, Task> action) =>
        result.Match<Task<Result<T>>>(
            success: _ => Task.FromResult(result),
            failure: async msg => { await action(msg); return result; }
        );

    // Bridge to SimpleResult (mirrors Tap(Action)/TapError(Action))
    public static Task<Result> TapAsync<T>(this Result<T> result, Func<Task> action) =>
        result.Match<Task<Result>>(
            success: async _ => { await action(); return new Success(); },
            failure: msg => Task.FromResult<Result>(new Failure(msg))
        );

    public static Task<Result> TapErrorAsync<T>(this Result<T> result, Func<Task> action) =>
        result.Match<Task<Result>>(
            success: _ => Task.FromResult<Result>(new Success()),
            failure: async msg => { await action(); return new Failure(msg); }
        );


    // Task → Tap
    public static async Task<Result<T>> Tap<T>(this Task<Result<T>> resultTask, Action<T> action) =>
        (await resultTask).Tap(action);

    public static async Task<Result<T>> TapError<T>(this Task<Result<T>> resultTask, Action<string> action) =>
        (await resultTask).TapError(action);

    public static async Task<Result> Tap<T>(this Task<Result<T>> resultTask, Action action) =>
        (await resultTask).Tap(action);

    public static async Task<Result> TapError<T>(this Task<Result<T>> resultTask, Action action) =>
        (await resultTask).TapError(action);


    // Task → Task
    public static async Task<Result<T>> TapAsync<T>(this Task<Result<T>> resultTask, Func<T, Task> action) =>
        await (await resultTask).TapAsync(action);

    public static async Task<Result<T>> TapErrorAsync<T>(this Task<Result<T>> resultTask, Func<string, Task> action) =>
        await (await resultTask).TapErrorAsync(action);

    public static async Task<Result> TapAsync<T>(this Task<Result<T>> resultTask, Func<Task> action) =>
        await (await resultTask).TapAsync(action);

    public static async Task<Result> TapErrorAsync<T>(this Task<Result<T>> resultTask, Func<Task> action) =>
        await (await resultTask).TapErrorAsync(action);
}
