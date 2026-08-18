using FunctionalTypes.Bridging;

namespace FunctionalTypes.TypedErrorResult;

public static class BindAsyncExtensions
{
    // Bind → Task
    public static Task<Result<TR, TError>> BindAsync<T, TR, TError>(this Result<T, TError> result, Func<T, Task<Result<TR, TError>>> binder) =>
        result.Match(
            success: binder,
            failure: error => Task.FromResult<Result<TR, TError>>(new Failure<TR, TError>(error))
        );

    public static Task<SimpleResult.Result> BindAsync<T, TError>(this Result<T, TError> result, Func<T, Task<SimpleResult.Result>> binder,
        Func<TError, string> errorSelector)
        => result.Match<Task<SimpleResult.Result>>(
            success: binder,
            failure: error => Task.FromResult<SimpleResult.Result>(new SimpleResult.Failure(errorSelector(error)))
        );

    public static Task<TypedResult.Result<TR>> BindAsync<T, TR, TError>(this Result<T, TError> result, Func<T, Task<TypedResult.Result<TR>>> binder,
        Func<TError, string> errorSelector)
        => result.Match<Task<TypedResult.Result<TR>>>(
            success: binder,
            failure: error => Task.FromResult<TypedResult.Result<TR>>(new TypedResult.Failure<TR>(errorSelector(error)))
        );


    // Task → Bind
    public static async Task<Result<TR, TError>> Bind<T, TR, TError>(this Task<Result<T, TError>> resultTask, Func<T, Result<TR, TError>> binder) =>
        (await resultTask).Bind(binder);

    public static async Task<SimpleResult.Result> Bind<T, TError>(this Task<Result<T, TError>> resultTask, Func<T, SimpleResult.Result> binder,
        Func<TError, string> errorSelector) =>
        (await resultTask).Bind(binder, errorSelector);

    public static async Task<TypedResult.Result<TR>> Bind<T, TR, TError>(this Task<Result<T, TError>> resultTask, Func<T, TypedResult.Result<TR>> binder,
        Func<TError, string> errorSelector) =>
        (await resultTask).Bind(binder, errorSelector);


    // Task → Task
    public static async Task<Result<TR, TError>> BindAsync<T, TR, TError>(this Task<Result<T, TError>> resultTask, Func<T, Task<Result<TR, TError>>> binder) =>
        await (await resultTask).BindAsync(binder);

    public static async Task<SimpleResult.Result> BindAsync<T, TError>(this Task<Result<T, TError>> resultTask, Func<T, Task<SimpleResult.Result>> binder,
        Func<TError, string> errorSelector) =>
        await (await resultTask).BindAsync(binder, errorSelector);

    public static async Task<TypedResult.Result<TR>> BindAsync<T, TR, TError>(this Task<Result<T, TError>> resultTask, Func<T, Task<TypedResult.Result<TR>>> binder,
        Func<TError, string> errorSelector) =>
        await (await resultTask).BindAsync(binder, errorSelector);
}
