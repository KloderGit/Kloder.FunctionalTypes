using FunctionalTypes.TypedResult;

namespace FunctionalTypes.Either;

public static class BindAsyncExtensions
{
    // Bind → Task
    public static Task<Either<TR, TRight>> BindLeftAsync<TLeft, TRight, TR>(this Either<TLeft, TRight> either,
        Func<TLeft, Task<Either<TR, TRight>>> binder) =>
        either.Match(
            onLeft: binder,
            onRight: right => Task.FromResult<Either<TR, TRight>>(new Right<TR, TRight>(right))
        );

    public static Task<Either<TLeft, TR>> BindRightAsync<TLeft, TRight, TR>(this Either<TLeft, TRight> either,
        Func<TRight, Task<Either<TLeft, TR>>> binder) =>
        either.Match(
            onLeft: left => Task.FromResult<Either<TLeft, TR>>(new Left<TLeft, TR>(left)),
            onRight: binder
        );


    // Task → Bind
    public static async Task<Either<TR, TRight>> BindLeft<TLeft, TRight, TR>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, Either<TR, TRight>> binder) =>
        (await eitherTask).BindLeft(binder);

    public static async Task<Either<TLeft, TR>> BindRight<TLeft, TRight, TR>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TRight, Either<TLeft, TR>> binder) =>
        (await eitherTask).BindRight(binder);


    // Task → Task
    public static async Task<Either<TR, TRight>> BindLeftAsync<TLeft, TRight, TR>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, Task<Either<TR, TRight>>> binder) =>
        await (await eitherTask).BindLeftAsync(binder);

    public static async Task<Either<TLeft, TR>> BindRightAsync<TLeft, TRight, TR>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TRight, Task<Either<TLeft, TR>>> binder) =>
        await (await eitherTask).BindRightAsync(binder);


    // Bridge into TypedResult.Result<TR> (async counterpart of BindExtensions.cs' Bind)
    public static Task<Result<TR>> BindAsync<TLeft, TRight, TR>(this Result<Either<TLeft, TRight>> result,
        Func<TLeft, Task<Result<TR>>> onLeft, Func<TRight, Task<Result<TR>>> onRight) =>
        result.BindAsync(either => either.Match(onLeft, onRight));

    public static async Task<Result<TR>> Bind<TLeft, TRight, TR>(this Task<Result<Either<TLeft, TRight>>> resultTask,
        Func<TLeft, Result<TR>> onLeft, Func<TRight, Result<TR>> onRight) =>
        (await resultTask).Bind(onLeft, onRight);

    public static async Task<Result<TR>> BindAsync<TLeft, TRight, TR>(
        this Task<Result<Either<TLeft, TRight>>> resultTask,
        Func<TLeft, Task<Result<TR>>> onLeft, Func<TRight, Task<Result<TR>>> onRight) =>
        await (await resultTask).BindAsync(onLeft, onRight);
}
