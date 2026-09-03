namespace FunctionalTypes.Maybe;

public sealed class None<T> : Maybe<T>
{
    public override bool IsSome => false;
    public override bool IsNone => true;

    public override Maybe<TR> Map<TR>(Func<T, TR> selector) => new None<TR>();

    public override Maybe<TR> Bind<TR>(Func<T, Maybe<TR>> binder) => new None<TR>();

    public override TR Match<TR>(Func<T, TR> some, Func<TR> none) => none();

    public override Maybe<T> Tap(Action<T> action) => this;

    public override Maybe<T> TapNone(Action action)
    {
        action();
        return this;
    }

    public override Maybe<T> Check(Predicate<T> predicate) => this;

    public override T GetValueOrDefault(T defaultValue) => defaultValue;

    public override void Deconstruct(out bool hasValue, out T? result)
    {
        hasValue = false;
        result = default;
    }
}
