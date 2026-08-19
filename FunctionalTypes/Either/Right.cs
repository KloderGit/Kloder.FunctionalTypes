namespace FunctionalTypes.Either;

public sealed class Right<TLeft, TRight>(TRight value) : Either<TLeft, TRight>
{
    public override bool IsLeft => false;
    public override bool IsRight => true;

    public override Either<TR, TRight> MapLeft<TR>(Func<TLeft, TR> selector) => new Right<TR, TRight>(value);
    public override Either<TLeft, TR> MapRight<TR>(Func<TRight, TR> selector) => new Right<TLeft, TR>(selector(value));

    public override Either<TR, TRight> BindLeft<TR>(Func<TLeft, Either<TR, TRight>> binder) => new Right<TR, TRight>(value);
    public override Either<TLeft, TR> BindRight<TR>(Func<TRight, Either<TLeft, TR>> binder) => binder(value);

    public override TR Match<TR>(Func<TLeft, TR> onLeft, Func<TRight, TR> onRight) => onRight(value);

    public override Either<TLeft, TRight> TapLeft(Action<TLeft> action) => this;

    public override Either<TLeft, TRight> TapRight(Action<TRight> action)
    {
        action(value);
        return this;
    }

    public override Either<TRight, TLeft> Swap() => new Left<TRight, TLeft>(value);

    public override void Deconstruct(out bool isLeft, out TLeft? left, out TRight? right)
    {
        isLeft = false;
        left = default;
        right = value;
    }
}
