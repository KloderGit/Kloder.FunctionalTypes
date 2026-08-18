using FunctionalTypes.TypedErrorResult;
using FunctionalTypes.TypedResult;

namespace FunctionalTypes.SimpleResult;

public static class BindAsyncExtensions
{
    // Bind → Task
    public static Task<Result> BindAsync(this Result result, Func<Task<Result>> binder) =>
        result.Match(
            success: binder,
            failure: msg => Task.FromResult<Result>(new Failure(msg))
        );
    
    public static Task<Result<TR>> BindAsync<TR>(this Result result, Func<Task<Result<TR>>> binder) =>
        result.Match(
            success: binder,
            failure: msg => Task.FromResult<Result<TR>>(new Failure<TR>(msg))
        );
    
    public static Task<Result<TR, TError>> BindAsync<TR, TError>(this Result result, Func<Task<Result<TR, TError>>> binder,
        Func<string, TError> errorSelector)
        => result.Match<Task<Result<TR, TError>>>(
            success: binder,
            failure: msg => Task.FromResult<Result<TR, TError>>(new Failure<TR, TError>(errorSelector(msg))
            ));
    
    
    // Task → Bind
    public static async Task<Result> Bind(this Task<Result> resultTask, Func<Result> binder) =>
        (await resultTask).Bind(binder);

    public static async Task<Result<TR>> Bind<TR>(this Task<Result> resultTask, Func<Result<TR>> binder) =>
        (await resultTask).Bind(binder);

    public static async Task<Result<TR, TError>> Bind<TR, TError>(this Task<Result> resultTask, Func<Result<TR, TError>> binder,
        Func<string, TError> errorSelector) =>
        (await resultTask).Match<Result<TR, TError>>(
            success: binder,
            failure: msg => new Failure<TR, TError>(errorSelector(msg))
        );


    // Task → Task
    public static async Task<Result> BindAsync(this Task<Result> resultTask, Func<Task<Result>> binder) =>
        await (await resultTask).BindAsync(binder);

    public static async Task<Result<TR>> BindAsync<TR>(this Task<Result> resultTask, Func<Task<Result<TR>>> binder) =>
        await (await resultTask).BindAsync(binder);

    public static async Task<Result<TR, TError>> BindAsync<TR, TError>(this Task<Result> resultTask, Func<Task<Result<TR, TError>>> binder,
        Func<string, TError> errorSelector) =>
        await (await resultTask).BindAsync(binder, errorSelector);

}