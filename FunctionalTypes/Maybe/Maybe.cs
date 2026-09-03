namespace FunctionalTypes.Maybe;

public abstract class Maybe<T>
{
    public static Maybe<T> Some(T value) => new Some<T>(value);
    public static Maybe<T> None() => new None<T>();

    public abstract bool IsSome { get; }
    public abstract bool IsNone { get; }

    public abstract Maybe<TR> Map<TR>(Func<T, TR> selector);
    public abstract Maybe<TR> Bind<TR>(Func<T, Maybe<TR>> binder);
    public abstract TR Match<TR>(Func<T, TR> some, Func<TR> none);
    public abstract Maybe<T> Tap(Action<T> action);
    public abstract Maybe<T> TapNone(Action action);
    public abstract Maybe<T> Check(Predicate<T> predicate);
    public abstract T GetValueOrDefault(T defaultValue);

    public abstract void Deconstruct(out bool hasValue, out T? value);
}
