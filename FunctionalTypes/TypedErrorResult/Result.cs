namespace FunctionalTypes.TypedErrorResult;

public abstract class Result<T, TError>
{
    public abstract Result<TR, TError> Map<TR>(Func<T, TR> selector);
    public abstract Result<TR, TError> Map<TR>(Func<TR> selector);
    public abstract Result<T, TError> Check(Predicate<T> predicate, Func<TError> errorFactory);
    public abstract TR Match<TR>(Func<T, TR> success, Func<TError, TR> failure);
    public abstract TR Match<TR>(Func<TR> success, Func<TError, TR> failure);
    public abstract Result<TR, TError> Bind<TR>(Func<T, Result<TR, TError>> binder);
    public abstract Result<T, TError> Tap(Action<T> action);
    public abstract Result<T, TError> Tap(Action action);
    public abstract Result<T, TError> TapError(Action<TError> action);
    public abstract Result<T, TError> TapError(Action action);
    
    public abstract void Deconstruct(out bool isSuccess, out TError? errorValue, out T? result);
}
