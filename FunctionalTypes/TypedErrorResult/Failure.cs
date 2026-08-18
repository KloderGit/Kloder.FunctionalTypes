namespace FunctionalTypes.TypedErrorResult;

public sealed class Failure<T, TError>(TError error) : Result<T, TError>
{
    public override Result<TR, TError> Map<TR>(Func<T, TR> selector)
        => new Failure<TR, TError>(error);

    public override Result<TR, TError> Map<TR>(Func<TR> selector)
        => new Failure<TR, TError>(error);

    public override Result<T, TError> Check(Predicate<T> predicate, Func<TError> errorFactory)
        => this;

    public override TR Match<TR>(Func<T, TR> success, Func<TError, TR> failure)
        => failure(error);

    public override TR Match<TR>(Func<TR> success, Func<TError, TR> failure)
        => failure(error);

    public override Result<TR, TError> Bind<TR>(Func<T, Result<TR, TError>> binder)
        => new Failure<TR, TError>(error);

    public override Result<T, TError> Tap(Action<T> action)
        => this;

    public override Result<T, TError> Tap(Action action)
        => this;

    public override Result<T, TError> TapError(Action<TError> action)
    {
        action(error);
        return this;
    }

    public override Result<T, TError> TapError(Action action)
    {
        action();
        return this;
    }

    public override void Deconstruct(out bool isSuccess, out TError? errorValue, out T? result)
    {
        isSuccess = false;
        errorValue = error;
        result = default;
    }
}
