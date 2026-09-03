using FunctionalTypes.Bridging;
using FunctionalTypes.TypedErrorResult;

namespace FunctionalTypes.TypedResult;

public static class MapAsyncExtensions
{
    // Map → Task
    public static Task<Result<TR>> MapAsync<T, TR>(this Result<T> result, Func<T, Task<TR>> selector) =>
        result.Match<Task<Result<TR>>>(
            success: async value => new Success<TR>(await selector(value)),
            failure: msg => Task.FromResult<Result<TR>>(new Failure<TR>(msg))
        );

    public static Task<Result<TR, TError>> MapAsync<T, TR, TError>(this Result<T> result, Func<T, Task<TR>> selector,
        Func<string, TError> errorSelector)
        => result.Match<Task<Result<TR, TError>>>(
            success: async value => new Success<TR, TError>(await selector(value)),
            failure: msg => Task.FromResult<Result<TR, TError>>(new Failure<TR, TError>(errorSelector(msg)))
        );


    // Task → Map
    public static async Task<Result<TR>> Map<T, TR>(this Task<Result<T>> resultTask, Func<T, TR> selector) =>
        (await resultTask).Map(selector);

    public static async Task<Result<TR, TError>> Map<T, TR, TError>(this Task<Result<T>> resultTask,
        Func<T, TR> selector, Func<string, TError> errorSelector) =>
        (await resultTask).Map(selector, errorSelector);


    // Task → Task
    public static async Task<Result<TR>> MapAsync<T, TR>(this Task<Result<T>> resultTask, Func<T, Task<TR>> selector) =>
        await (await resultTask).MapAsync(selector);

    public static async Task<Result<TR, TError>> MapAsync<T, TR, TError>(this Task<Result<T>> resultTask,
        Func<T, Task<TR>> selector, Func<string, TError> errorSelector) =>
        await (await resultTask).MapAsync(selector, errorSelector);
}
