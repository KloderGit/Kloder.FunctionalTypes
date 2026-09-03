namespace FunctionalTypes.Either;

public static class TapAsyncExtensions
{
    // Tap → Task
    public static Task<Either<TLeft, TRight>> TapLeftAsync<TLeft, TRight>(this Either<TLeft, TRight> either,
        Func<TLeft, Task> action) =>
        either.Match<Task<Either<TLeft, TRight>>>(
            onLeft: async left => { await action(left); return either; },
            onRight: _ => Task.FromResult(either)
        );

    public static Task<Either<TLeft, TRight>> TapRightAsync<TLeft, TRight>(this Either<TLeft, TRight> either,
        Func<TRight, Task> action) =>
        either.Match<Task<Either<TLeft, TRight>>>(
            onLeft: _ => Task.FromResult(either),
            onRight: async right => { await action(right); return either; }
        );


    // Task → Tap
    public static async Task<Either<TLeft, TRight>> TapLeft<TLeft, TRight>(
        this Task<Either<TLeft, TRight>> eitherTask, Action<TLeft> action) =>
        (await eitherTask).TapLeft(action);

    public static async Task<Either<TLeft, TRight>> TapRight<TLeft, TRight>(
        this Task<Either<TLeft, TRight>> eitherTask, Action<TRight> action) =>
        (await eitherTask).TapRight(action);


    // Task → Task
    public static async Task<Either<TLeft, TRight>> TapLeftAsync<TLeft, TRight>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, Task> action) =>
        await (await eitherTask).TapLeftAsync(action);

    public static async Task<Either<TLeft, TRight>> TapRightAsync<TLeft, TRight>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TRight, Task> action) =>
        await (await eitherTask).TapRightAsync(action);
}
