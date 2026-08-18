using FunctionalTypes.Bridging;
using FunctionalTypes.SimpleResult;
using FunctionalTypes.TypedErrorResult;

namespace FunctionalTypes.TypedResult;

public static class BindAsyncExtensions
{
    // Bind → Task
    public static Task<Result<TR>> BindAsync<T, TR>(this Result<T> result, Func<T, Task<Result<TR>>> binder) =>
        result.Match(
            success: binder,
            failure: msg => Task.FromResult<Result<TR>>(new Failure<TR>(msg))
        );

    public static Task<Result> BindAsync<T>(this Result<T> result, Func<Task<Result>> binder) =>
        result.Match(
            success: binder,
            failure: msg => Task.FromResult<Result>(new Failure(msg))
        );

    public static Task<Result<TR, TError>> BindAsync<T, TR, TError>(this Result<T> result, Func<T, Task<Result<TR, TError>>> binder,
        Func<string, TError> errorSelector)
        => result.Match<Task<Result<TR, TError>>>(
            success: binder,
            failure: msg => Task.FromResult<Result<TR, TError>>(new Failure<TR, TError>(errorSelector(msg)))
        );


    // Task → Bind
    public static async Task<Result<TR>> Bind<T, TR>(this Task<Result<T>> resultTask, Func<T, Result<TR>> binder) =>
        (await resultTask).Bind(binder);

    public static async Task<Result> Bind<T>(this Task<Result<T>> resultTask, Func<Result> binder) =>
        (await resultTask).Bind(binder);

    public static async Task<Result<TR, TError>> Bind<T, TR, TError>(this Task<Result<T>> resultTask, Func<T, Result<TR, TError>> binder,
        Func<string, TError> errorSelector) =>
        (await resultTask).Bind(binder, errorSelector);


    // Task → Task
    public static async Task<Result<TR>> BindAsync<T, TR>(this Task<Result<T>> resultTask, Func<T, Task<Result<TR>>> binder) =>
        await (await resultTask).BindAsync(binder);

    public static async Task<Result> BindAsync<T>(this Task<Result<T>> resultTask, Func<Task<Result>> binder) =>
        await (await resultTask).BindAsync(binder);

    public static async Task<Result<TR, TError>> BindAsync<T, TR, TError>(this Task<Result<T>> resultTask, Func<T, Task<Result<TR, TError>>> binder,
        Func<string, TError> errorSelector) =>
        await (await resultTask).BindAsync(binder, errorSelector);
}
