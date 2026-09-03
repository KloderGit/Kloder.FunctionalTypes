namespace FunctionalTypes.TypedErrorResult;

public static class CheckAsyncExtensions
{
    // Check → Task
    public static Task<Result<T, TError>> CheckAsync<T, TError>(this Result<T, TError> result,
        Func<T, Task<bool>> predicate, Func<TError> errorFactory) =>
        result.Match<Task<Result<T, TError>>>(
            success: async value => await predicate(value) ? result : new Failure<T, TError>(errorFactory()),
            failure: _ => Task.FromResult(result)
        );


    // Task → Check
    public static async Task<Result<T, TError>> Check<T, TError>(this Task<Result<T, TError>> resultTask,
        Predicate<T> predicate, Func<TError> errorFactory) =>
        (await resultTask).Check(predicate, errorFactory);


    // Task → Task
    public static async Task<Result<T, TError>> CheckAsync<T, TError>(this Task<Result<T, TError>> resultTask,
        Func<T, Task<bool>> predicate, Func<TError> errorFactory) =>
        await (await resultTask).CheckAsync(predicate, errorFactory);
}
