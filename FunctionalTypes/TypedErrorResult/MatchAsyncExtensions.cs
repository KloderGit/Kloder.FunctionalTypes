namespace FunctionalTypes.TypedErrorResult;

public static class MatchAsyncExtensions
{
    // Match → Task
    public static Task<TR> MatchAsync<T, TError, TR>(this Result<T, TError> result, Func<T, Task<TR>> success,
        Func<TError, Task<TR>> failure) =>
        result.Match(success, failure);

    public static Task<TR> MatchAsync<T, TError, TR>(this Result<T, TError> result, Func<Task<TR>> success,
        Func<TError, Task<TR>> failure) =>
        result.Match(success, failure);


    // Task → Match
    public static async Task<TR> Match<T, TError, TR>(this Task<Result<T, TError>> resultTask, Func<T, TR> success,
        Func<TError, TR> failure) =>
        (await resultTask).Match(success, failure);

    public static async Task<TR> Match<T, TError, TR>(this Task<Result<T, TError>> resultTask, Func<TR> success,
        Func<TError, TR> failure) =>
        (await resultTask).Match(success, failure);


    // Task → Task
    public static async Task<TR> MatchAsync<T, TError, TR>(this Task<Result<T, TError>> resultTask,
        Func<T, Task<TR>> success, Func<TError, Task<TR>> failure) =>
        await (await resultTask).MatchAsync(success, failure);

    public static async Task<TR> MatchAsync<T, TError, TR>(this Task<Result<T, TError>> resultTask,
        Func<Task<TR>> success, Func<TError, Task<TR>> failure) =>
        await (await resultTask).MatchAsync(success, failure);
}
