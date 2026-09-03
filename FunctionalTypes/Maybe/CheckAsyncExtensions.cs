namespace FunctionalTypes.Maybe;

public static class CheckAsyncExtensions
{
    // Check → Task
    public static Task<Maybe<T>> CheckAsync<T>(this Maybe<T> maybe, Func<T, Task<bool>> predicate) =>
        maybe.Match<Task<Maybe<T>>>(
            some: async value => await predicate(value) ? maybe : new None<T>(),
            none: () => Task.FromResult(maybe)
        );


    // Task → Check
    public static async Task<Maybe<T>> Check<T>(this Task<Maybe<T>> maybeTask, Predicate<T> predicate) =>
        (await maybeTask).Check(predicate);


    // Task → Task
    public static async Task<Maybe<T>> CheckAsync<T>(this Task<Maybe<T>> maybeTask, Func<T, Task<bool>> predicate) =>
        await (await maybeTask).CheckAsync(predicate);
}
