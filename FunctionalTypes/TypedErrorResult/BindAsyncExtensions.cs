namespace FunctionalTypes.TypedErrorResult;

public static class BindAsyncExtensions
{
    // Bind → Task
    public static Task<Result<TR, TError>> BindAsync<T, TR, TError>(this Result<T, TError> result, Func<T, Task<Result<TR, TError>>> binder) =>
        result.Match(
            success: binder,
            failure: error => Task.FromResult<Result<TR, TError>>(new Failure<TR, TError>(error))
        );


    // Task → Bind
    public static async Task<Result<TR, TError>> Bind<T, TR, TError>(this Task<Result<T, TError>> resultTask, Func<T, Result<TR, TError>> binder) =>
        (await resultTask).Bind(binder);


    // Task → Task
    public static async Task<Result<TR, TError>> BindAsync<T, TR, TError>(this Task<Result<T, TError>> resultTask, Func<T, Task<Result<TR, TError>>> binder) =>
        await (await resultTask).BindAsync(binder);
}
