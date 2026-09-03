namespace FunctionalTypes.TypedResult;

public static class CheckAsyncExtensions
{
    // Check → Task
    public static Task<Result<T>> CheckAsync<T>(this Result<T> result, Func<T, Task<bool>> predicate, string? message = null) =>
        result.Match<Task<Result<T>>>(
            success: async value => await predicate(value) ? result : new Failure<T>(message ?? "Check failed"),
            failure: _ => Task.FromResult(result)
        );


    // Task → Check
    public static async Task<Result<T>> Check<T>(this Task<Result<T>> resultTask, Predicate<T> predicate, string? message = null) =>
        (await resultTask).Check(predicate, message);


    // Task → Task
    public static async Task<Result<T>> CheckAsync<T>(this Task<Result<T>> resultTask, Func<T, Task<bool>> predicate, string? message = null) =>
        await (await resultTask).CheckAsync(predicate, message);
}
