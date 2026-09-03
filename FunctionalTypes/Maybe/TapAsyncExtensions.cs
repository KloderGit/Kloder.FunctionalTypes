namespace FunctionalTypes.Maybe;

public static class TapAsyncExtensions
{
    // Tap → Task
    public static Task<Maybe<T>> TapAsync<T>(this Maybe<T> maybe, Func<T, Task> action) =>
        maybe.Match<Task<Maybe<T>>>(
            some: async value => { await action(value); return maybe; },
            none: () => Task.FromResult(maybe)
        );

    public static Task<Maybe<T>> TapNoneAsync<T>(this Maybe<T> maybe, Func<Task> action) =>
        maybe.Match<Task<Maybe<T>>>(
            some: _ => Task.FromResult(maybe),
            none: async () => { await action(); return maybe; }
        );


    // Task → Tap
    public static async Task<Maybe<T>> Tap<T>(this Task<Maybe<T>> maybeTask, Action<T> action) =>
        (await maybeTask).Tap(action);

    public static async Task<Maybe<T>> TapNone<T>(this Task<Maybe<T>> maybeTask, Action action) =>
        (await maybeTask).TapNone(action);


    // Task → Task
    public static async Task<Maybe<T>> TapAsync<T>(this Task<Maybe<T>> maybeTask, Func<T, Task> action) =>
        await (await maybeTask).TapAsync(action);

    public static async Task<Maybe<T>> TapNoneAsync<T>(this Task<Maybe<T>> maybeTask, Func<Task> action) =>
        await (await maybeTask).TapNoneAsync(action);
}
