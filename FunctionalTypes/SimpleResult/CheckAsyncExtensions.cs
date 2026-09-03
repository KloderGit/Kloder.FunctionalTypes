namespace FunctionalTypes.SimpleResult;

public static class CheckAsyncExtensions
{
    // Check → Task
    public static Task<Result> CheckAsync(this Result result, Func<Task<bool>> predicate, string? message = null) =>
        result.Match<Task<Result>>(
            success: async () => await predicate() ? result : new Failure(message ?? "Check failed"),
            failure: _ => Task.FromResult(result)
        );


    // Task → Check
    public static async Task<Result> Check(this Task<Result> resultTask, Func<bool> predicate, string? message = null) =>
        (await resultTask).Check(predicate, message);


    // Task → Task
    public static async Task<Result> CheckAsync(this Task<Result> resultTask, Func<Task<bool>> predicate, string? message = null) =>
        await (await resultTask).CheckAsync(predicate, message);
}
