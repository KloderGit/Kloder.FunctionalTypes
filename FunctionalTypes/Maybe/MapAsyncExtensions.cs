namespace FunctionalTypes.Maybe;

public static class MapAsyncExtensions
{
    // Map → Task
    public static Task<Maybe<TR>> MapAsync<T, TR>(this Maybe<T> maybe, Func<T, Task<TR>> selector) =>
        maybe.Match<Task<Maybe<TR>>>(
            some: async value => new Some<TR>(await selector(value)),
            none: () => Task.FromResult<Maybe<TR>>(new None<TR>())
        );


    // Task → Map
    public static async Task<Maybe<TR>> Map<T, TR>(this Task<Maybe<T>> maybeTask, Func<T, TR> selector) =>
        (await maybeTask).Map(selector);


    // Task → Task
    public static async Task<Maybe<TR>> MapAsync<T, TR>(this Task<Maybe<T>> maybeTask, Func<T, Task<TR>> selector) =>
        await (await maybeTask).MapAsync(selector);
}
