namespace FunctionalTypes.Maybe;

public sealed class Nothing<T> : Maybe<T>
{
    public override bool IsJust => false;
    public override bool IsNothing => true;

    public override Maybe<TR> Map<TR>(Func<T, TR> selector) => new Nothing<TR>();

    public override Maybe<TR> Bind<TR>(Func<T, Maybe<TR>> binder) => new Nothing<TR>();

    public override TR Match<TR>(Func<T, TR> just, Func<TR> nothing) => nothing();

    public override Maybe<T> Tap(Action<T> action) => this;

    public override Maybe<T> TapNothing(Action action)
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
