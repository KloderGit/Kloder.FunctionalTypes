namespace FunctionalTypes.TypedErrorResult;

public sealed class Success<T, TError>(T value) : Result<T, TError>
{
    public override Result<TR, TError> Map<TR>(Func<T, TR> selector)
        => new Success<TR, TError>(selector(value));

    public override Result<T, TError> Check(Predicate<T> predicate, Func<TError> errorFactory)
        => predicate(value) ? this : new Failure<T, TError>(errorFactory());

    public override TR Match<TR>(Func<T, TR> success, Func<TError, TR> failure)
        => success(value);

    public override TR Match<TR>(Func<TR> success, Func<TError, TR> failure)
        => success();

    public override Result<TR, TError> Bind<TR>(Func<T, Result<TR, TError>> binder)
        => binder(value);

    public override Result<T, TError> Tap(Action<T> action)
    {
        action(value);
        return this;
    }

    public override Result<T, TError> TapError(Action<TError> action)
        => this;

    public override void Deconstruct(out bool isSuccess, out TError? errorValue, out T? result)
    {
        isSuccess = true;
        errorValue = default;
        result = value;
    }
}
