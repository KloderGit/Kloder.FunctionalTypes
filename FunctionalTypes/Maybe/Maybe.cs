namespace FunctionalTypes.Maybe;

public abstract class Maybe<T>
{
    public static Maybe<T> Just(T value) => new Just<T>(value);
    public static Maybe<T> Nothing() => new Nothing<T>();

    public abstract bool IsJust { get; }
    public abstract bool IsNothing { get; }

    public abstract Maybe<TR> Map<TR>(Func<T, TR> selector);
    public abstract Maybe<TR> Bind<TR>(Func<T, Maybe<TR>> binder);
    public abstract TR Match<TR>(Func<T, TR> just, Func<TR> nothing);
    public abstract Maybe<T> Tap(Action<T> action);
    public abstract Maybe<T> TapNothing(Action action);
    public abstract Maybe<T> Check(Predicate<T> predicate);
    public abstract T GetValueOrDefault(T defaultValue);

    public abstract void Deconstruct(out bool hasValue, out T? value);
}
