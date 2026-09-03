namespace FunctionalTypes.SimpleResult;

public static class TapAsyncExtensions
{
    // Tap → Task
    public static Task<Result> TapAsync(this Result result, Func<Task> action) =>
        result.Match<Task<Result>>(
            success: async () => { await action(); return result; },
            failure: _ => Task.FromResult(result)
        );

    public static Task<Result> TapErrorAsync(this Result result, Func<Task> action) =>
        result.Match<Task<Result>>(
            success: () => Task.FromResult(result),
            failure: async _ => { await action(); return result; }
        );


    // Task → Tap
    public static async Task<Result> Tap(this Task<Result> resultTask, Action action) =>
        (await resultTask).Tap(action);

    public static async Task<Result> TapError(this Task<Result> resultTask, Action action) =>
        (await resultTask).TapError(action);


    // Task → Task
    public static async Task<Result> TapAsync(this Task<Result> resultTask, Func<Task> action) =>
        await (await resultTask).TapAsync(action);

    public static async Task<Result> TapErrorAsync(this Task<Result> resultTask, Func<Task> action) =>
        await (await resultTask).TapErrorAsync(action);
}
