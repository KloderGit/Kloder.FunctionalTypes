namespace FunctionalTypes.Maybe;

public static class TapAsyncExtensions
{
    // Tap → Task
    public static Task<Maybe<T>> TapAsync<T>(this Maybe<T> maybe, Func<T, Task> action) =>
        maybe.Match<Task<Maybe<T>>>(
            just: async value => { await action(value); return maybe; },
            nothing: () => Task.FromResult(maybe)
        );

    public static Task<Maybe<T>> TapNothingAsync<T>(this Maybe<T> maybe, Func<Task> action) =>
        maybe.Match<Task<Maybe<T>>>(
            just: _ => Task.FromResult(maybe),
            nothing: async () => { await action(); return maybe; }
        );


    // Task → Tap
    public static async Task<Maybe<T>> Tap<T>(this Task<Maybe<T>> maybeTask, Action<T> action) =>
        (await maybeTask).Tap(action);

    public static async Task<Maybe<T>> TapNothing<T>(this Task<Maybe<T>> maybeTask, Action action) =>
        (await maybeTask).TapNothing(action);


    // Task → Task
    public static async Task<Maybe<T>> TapAsync<T>(this Task<Maybe<T>> maybeTask, Func<T, Task> action) =>
        await (await maybeTask).TapAsync(action);

    public static async Task<Maybe<T>> TapNothingAsync<T>(this Task<Maybe<T>> maybeTask, Func<Task> action) =>
        await (await maybeTask).TapNothingAsync(action);
}
