namespace FunctionalTypes.Either;

public abstract class Either<TLeft, TRight>
{
    public static Either<TLeft, TRight> Left(TLeft value) => new Left<TLeft, TRight>(value);
    public static Either<TLeft, TRight> Right(TRight value) => new Right<TLeft, TRight>(value);

    public abstract bool IsLeft { get; }
    public abstract bool IsRight { get; }

    public abstract Either<TR, TRight> MapLeft<TR>(Func<TLeft, TR> selector);
    public abstract Either<TLeft, TR> MapRight<TR>(Func<TRight, TR> selector);

    public abstract Either<TR, TRight> BindLeft<TR>(Func<TLeft, Either<TR, TRight>> binder);
    public abstract Either<TLeft, TR> BindRight<TR>(Func<TRight, Either<TLeft, TR>> binder);

    public abstract TR Match<TR>(Func<TLeft, TR> onLeft, Func<TRight, TR> onRight);

    public abstract Either<TLeft, TRight> TapLeft(Action<TLeft> action);
    public abstract Either<TLeft, TRight> TapRight(Action<TRight> action);

    public abstract Either<TRight, TLeft> Swap();

    public abstract void Deconstruct(out bool isLeft, out TLeft? left, out TRight? right);
}
