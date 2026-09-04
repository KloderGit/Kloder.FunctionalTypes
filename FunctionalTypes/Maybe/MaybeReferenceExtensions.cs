namespace FunctionalTypes.Maybe;

public static class MaybeReferenceExtensions
{
    public static Maybe<T> ToMaybe<T>(this T? value) where T : class =>
        value is null ? Maybe<T>.Nothing() : Maybe<T>.Just(value);
}
