using FunctionalTypes.Bridging;
using FunctionalTypes.TypedErrorResult;
using FunctionalTypes.TypedResult;

namespace FunctionalTypes.SimpleResult;

public static class MapAsyncExtensions
{
    // Map → Task
    public static Task<Result<TR>> MapAsync<TR>(this Result result, Func<Task<TR>> selector) =>
        result.Match<Task<Result<TR>>>(
            success: async () => new Success<TR>(await selector()),
            failure: msg => Task.FromResult<Result<TR>>(new Failure<TR>(msg))
        );

    public static Task<Result<TR, TError>> MapAsync<TR, TError>(this Result result, Func<Task<TR>> selector,
        Func<string, TError> errorSelector)
        => result.Match<Task<Result<TR, TError>>>(
            success: async () => new Success<TR, TError>(await selector()),
            failure: msg => Task.FromResult<Result<TR, TError>>(new Failure<TR, TError>(errorSelector(msg)))
        );


    // Task → Map
    public static async Task<Result<TR>> Map<TR>(this Task<Result> resultTask, Func<TR> selector) =>
        (await resultTask).Map(selector);

    public static async Task<Result<TR, TError>> Map<TR, TError>(this Task<Result> resultTask, Func<TR> selector,
        Func<string, TError> errorSelector) =>
        (await resultTask).Map(selector, errorSelector);


    // Task → Task
    public static async Task<Result<TR>> MapAsync<TR>(this Task<Result> resultTask, Func<Task<TR>> selector) =>
        await (await resultTask).MapAsync(selector);

    public static async Task<Result<TR, TError>> MapAsync<TR, TError>(this Task<Result> resultTask,
        Func<Task<TR>> selector, Func<string, TError> errorSelector) =>
        await (await resultTask).MapAsync(selector, errorSelector);
}
