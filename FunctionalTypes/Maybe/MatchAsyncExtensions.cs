namespace FunctionalTypes.Maybe;

public static class MatchAsyncExtensions
{
    // Match → Task
    public static Task<TR> MatchAsync<T, TR>(this Maybe<T> maybe, Func<T, Task<TR>> some, Func<Task<TR>> none) =>
        maybe.Match(some, none);


    // Task → Match
    public static async Task<TR> Match<T, TR>(this Task<Maybe<T>> maybeTask, Func<T, TR> some, Func<TR> none) =>
        (await maybeTask).Match(some, none);


    // Task → Task
    public static async Task<TR> MatchAsync<T, TR>(this Task<Maybe<T>> maybeTask, Func<T, Task<TR>> some, Func<Task<TR>> none) =>
        await (await maybeTask).MatchAsync(some, none);
}
