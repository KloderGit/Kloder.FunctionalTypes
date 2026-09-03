using FunctionalTypes.Bridging;

namespace FunctionalTypes.TypedErrorResult;

public static class MapAsyncExtensions
{
    // Map → Task
    public static Task<Result<TR, TError>> MapAsync<T, TR, TError>(this Result<T, TError> result,
        Func<T, Task<TR>> selector) =>
        result.Match<Task<Result<TR, TError>>>(
            success: async value => new Success<TR, TError>(await selector(value)),
            failure: error => Task.FromResult<Result<TR, TError>>(new Failure<TR, TError>(error))
        );

    public static Task<TypedResult.Result<TR>> MapAsync<T, TR, TError>(this Result<T, TError> result,
        Func<T, Task<TR>> selector, Func<TError, string> errorSelector)
        => result.Match<Task<TypedResult.Result<TR>>>(
            success: async value => new TypedResult.Success<TR>(await selector(value)),
            failure: error => Task.FromResult<TypedResult.Result<TR>>(new TypedResult.Failure<TR>(errorSelector(error)))
        );


    // Task → Map
    public static async Task<Result<TR, TError>> Map<T, TR, TError>(this Task<Result<T, TError>> resultTask,
        Func<T, TR> selector) =>
        (await resultTask).Map(selector);

    public static async Task<TypedResult.Result<TR>> Map<T, TR, TError>(this Task<Result<T, TError>> resultTask,
        Func<T, TR> selector, Func<TError, string> errorSelector) =>
        (await resultTask).Map(selector, errorSelector);


    // Task → Task
    public static async Task<Result<TR, TError>> MapAsync<T, TR, TError>(this Task<Result<T, TError>> resultTask,
        Func<T, Task<TR>> selector) =>
        await (await resultTask).MapAsync(selector);

    public static async Task<TypedResult.Result<TR>> MapAsync<T, TR, TError>(this Task<Result<T, TError>> resultTask,
        Func<T, Task<TR>> selector, Func<TError, string> errorSelector) =>
        await (await resultTask).MapAsync(selector, errorSelector);
}
