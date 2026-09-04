namespace FunctionalTypes.Maybe;

public static class MaybeValueExtensions
{
    public static Maybe<T> ToMaybe<T>(this T value) where T : struct =>
        Maybe<T>.Just(value);

    public static Maybe<T> ToMaybe<T>(this T? value) where T : struct =>
        value is null ? Maybe<T>.Nothing() : Maybe<T>.Just(value.Value);
}
