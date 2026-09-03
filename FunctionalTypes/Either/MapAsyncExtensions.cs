namespace FunctionalTypes.Either;

public static class MapAsyncExtensions
{
    // Map → Task
    public static Task<Either<TR, TRight>> MapLeftAsync<TLeft, TRight, TR>(this Either<TLeft, TRight> either,
        Func<TLeft, Task<TR>> selector) =>
        either.Match<Task<Either<TR, TRight>>>(
            onLeft: async left => new Left<TR, TRight>(await selector(left)),
            onRight: right => Task.FromResult<Either<TR, TRight>>(new Right<TR, TRight>(right))
        );

    public static Task<Either<TLeft, TR>> MapRightAsync<TLeft, TRight, TR>(this Either<TLeft, TRight> either,
        Func<TRight, Task<TR>> selector) =>
        either.Match<Task<Either<TLeft, TR>>>(
            onLeft: left => Task.FromResult<Either<TLeft, TR>>(new Left<TLeft, TR>(left)),
            onRight: async right => new Right<TLeft, TR>(await selector(right))
        );


    // Task → Map
    public static async Task<Either<TR, TRight>> MapLeft<TLeft, TRight, TR>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, TR> selector) =>
        (await eitherTask).MapLeft(selector);

    public static async Task<Either<TLeft, TR>> MapRight<TLeft, TRight, TR>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TRight, TR> selector) =>
        (await eitherTask).MapRight(selector);


    // Task → Task
    public static async Task<Either<TR, TRight>> MapLeftAsync<TLeft, TRight, TR>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, Task<TR>> selector) =>
        await (await eitherTask).MapLeftAsync(selector);

    public static async Task<Either<TLeft, TR>> MapRightAsync<TLeft, TRight, TR>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TRight, Task<TR>> selector) =>
        await (await eitherTask).MapRightAsync(selector);
}
