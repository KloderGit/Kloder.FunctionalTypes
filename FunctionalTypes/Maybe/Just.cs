namespace FunctionalTypes.Maybe;

public sealed class Just<T>(T value) : Maybe<T>
{
    public override bool IsJust => true;
    public override bool IsNothing => false;

    public override Maybe<TR> Map<TR>(Func<T, TR> selector) => new Just<TR>(selector(value));

    public override Maybe<TR> Bind<TR>(Func<T, Maybe<TR>> binder) => binder(value);

    public override TR Match<TR>(Func<T, TR> just, Func<TR> nothing) => just(value);

    public override Maybe<T> Tap(Action<T> action)
    {
        action(value);
        return this;
    }

    public override Maybe<T> TapNothing(Action action) => this;

    public override Maybe<T> Check(Predicate<T> predicate) => predicate(value) ? this : new Nothing<T>();

    public override T GetValueOrDefault(T defaultValue) => value;

    public override void Deconstruct(out bool hasValue, out T? result)
    {
        hasValue = true;
        result = value;
    }
}
