namespace FunctionalTypes.Maybe;

public sealed class Some<T>(T value) : Maybe<T>
{
    public override bool IsSome => true;
    public override bool IsNone => false;

    public override Maybe<TR> Map<TR>(Func<T, TR> selector) => new Some<TR>(selector(value));

    public override Maybe<TR> Bind<TR>(Func<T, Maybe<TR>> binder) => binder(value);

    public override TR Match<TR>(Func<T, TR> some, Func<TR> none) => some(value);

    public override Maybe<T> Tap(Action<T> action)
    {
        action(value);
        return this;
    }

    public override Maybe<T> TapNone(Action action) => this;

    public override Maybe<T> Check(Predicate<T> predicate) => predicate(value) ? this : new None<T>();

    public override T GetValueOrDefault(T defaultValue) => value;

    public override void Deconstruct(out bool hasValue, out T? result)
    {
        hasValue = true;
        result = value;
    }
}
