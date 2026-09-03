namespace FunctionalTypes.SimpleResult;

public static class MatchAsyncExtensions
{
    // Match → Task
    public static Task<TR> MatchAsync<TR>(this Result result, Func<Task<TR>> success, Func<string, Task<TR>> failure) =>
        result.Match(success, failure);


    // Task → Match
    public static async Task<TR> Match<TR>(this Task<Result> resultTask, Func<TR> success, Func<string, TR> failure) =>
        (await resultTask).Match(success, failure);


    // Task → Task
    public static async Task<TR> MatchAsync<TR>(this Task<Result> resultTask, Func<Task<TR>> success, Func<string, Task<TR>> failure) =>
        await (await resultTask).MatchAsync(success, failure);
}
