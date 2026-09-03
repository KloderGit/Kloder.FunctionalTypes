namespace FunctionalTypes.Either;

public static class MatchAsyncExtensions
{
    // Match → Task
    public static Task<TR> MatchAsync<TLeft, TRight, TR>(this Either<TLeft, TRight> either,
        Func<TLeft, Task<TR>> onLeft, Func<TRight, Task<TR>> onRight) =>
        either.Match(onLeft, onRight);


    // Task → Match
    public static async Task<TR> Match<TLeft, TRight, TR>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, TR> onLeft, Func<TRight, TR> onRight) =>
        (await eitherTask).Match(onLeft, onRight);


    // Task → Task
    public static async Task<TR> MatchAsync<TLeft, TRight, TR>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, Task<TR>> onLeft, Func<TRight, Task<TR>> onRight) =>
        await (await eitherTask).MatchAsync(onLeft, onRight);
}
