namespace FunctionalTypes.Either;

public sealed class Left<TLeft, TRight>(TLeft value) : Either<TLeft, TRight>
{
    public override bool IsLeft => true;
    public override bool IsRight => false;

    public override Either<TR, TRight> MapLeft<TR>(Func<TLeft, TR> selector) => new Left<TR, TRight>(selector(value));
    public override Either<TLeft, TR> MapRight<TR>(Func<TRight, TR> selector) => new Left<TLeft, TR>(value);

    public override Either<TR, TRight> BindLeft<TR>(Func<TLeft, Either<TR, TRight>> binder) => binder(value);
    public override Either<TLeft, TR> BindRight<TR>(Func<TRight, Either<TLeft, TR>> binder) => new Left<TLeft, TR>(value);

    public override TR Match<TR>(Func<TLeft, TR> onLeft, Func<TRight, TR> onRight) => onLeft(value);

    public override Either<TLeft, TRight> TapLeft(Action<TLeft> action)
    {
        action(value);
        return this;
    }

    public override Either<TLeft, TRight> TapRight(Action<TRight> action) => this;

    public override Either<TRight, TLeft> Swap() => new Right<TRight, TLeft>(value);

    public override void Deconstruct(out bool isLeft, out TLeft? left, out TRight? right)
    {
        isLeft = true;
        left = value;
        right = default;
    }
}
