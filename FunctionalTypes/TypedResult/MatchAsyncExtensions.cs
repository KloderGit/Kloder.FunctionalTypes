namespace FunctionalTypes.TypedResult;

public static class MatchAsyncExtensions
{
    // Match → Task
    public static Task<TR> MatchAsync<T, TR>(this Result<T> result, Func<T, Task<TR>> success, Func<string, Task<TR>> failure) =>
        result.Match(success, failure);

    public static Task<TR> MatchAsync<T, TR>(this Result<T> result, Func<Task<TR>> success, Func<string, Task<TR>> failure) =>
        result.Match(success, failure);


    // Task → Match
    public static async Task<TR> Match<T, TR>(this Task<Result<T>> resultTask, Func<T, TR> success, Func<string, TR> failure) =>
        (await resultTask).Match(success, failure);

    public static async Task<TR> Match<T, TR>(this Task<Result<T>> resultTask, Func<TR> success, Func<string, TR> failure) =>
        (await resultTask).Match(success, failure);


    // Task → Task
    public static async Task<TR> MatchAsync<T, TR>(this Task<Result<T>> resultTask, Func<T, Task<TR>> success, Func<string, Task<TR>> failure) =>
        await (await resultTask).MatchAsync(success, failure);

    public static async Task<TR> MatchAsync<T, TR>(this Task<Result<T>> resultTask, Func<Task<TR>> success, Func<string, Task<TR>> failure) =>
        await (await resultTask).MatchAsync(success, failure);
}
