namespace FunctionalTypes.Maybe;

public static class BindAsyncExtensions
{
    // Bind → Task
    public static Task<Maybe<TR>> BindAsync<T, TR>(this Maybe<T> maybe, Func<T, Task<Maybe<TR>>> binder) =>
        maybe.Match(
            some: binder,
            none: () => Task.FromResult<Maybe<TR>>(new None<TR>())
        );


    // Task → Bind
    public static async Task<Maybe<TR>> Bind<T, TR>(this Task<Maybe<T>> maybeTask, Func<T, Maybe<TR>> binder) =>
        (await maybeTask).Bind(binder);


    // Task → Task
    public static async Task<Maybe<TR>> BindAsync<T, TR>(this Task<Maybe<T>> maybeTask, Func<T, Task<Maybe<TR>>> binder) =>
        await (await maybeTask).BindAsync(binder);
}
