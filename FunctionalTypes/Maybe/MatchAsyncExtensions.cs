namespace FunctionalTypes.Maybe;

public static class MatchAsyncExtensions
{
    // Match → Task
    public static Task<TR> MatchAsync<T, TR>(this Maybe<T> maybe, Func<T, Task<TR>> just, Func<Task<TR>> nothing) =>
        maybe.Match(just, nothing);


    // Task → Match
    public static async Task<TR> Match<T, TR>(this Task<Maybe<T>> maybeTask, Func<T, TR> just, Func<TR> nothing) =>
        (await maybeTask).Match(just, nothing);


    // Task → Task
    public static async Task<TR> MatchAsync<T, TR>(this Task<Maybe<T>> maybeTask, Func<T, Task<TR>> just, Func<Task<TR>> nothing) =>
        await (await maybeTask).MatchAsync(just, nothing);
}
