using FunctionalTypes.SimpleResult;

namespace FunctionalTypes.TypedResult;

public abstract class Result<T>
{
    public abstract Result<TR> Map<TR>(Func<T, TR> selector);
    public abstract Result<TR> Map<TR>(Func<TR> selector);

    public abstract Result<T> Check(Predicate<T> predicate, string? message = null);

    public abstract TR Match<TR>(Func<T, TR> success, Func<string, TR> failure);
    public abstract TR Match<TR>(Func<TR> success, Func<string, TR> failure);

    public abstract Result<TR> Bind<TR>(Func<T, Result<TR>> binder);
    
    public abstract Result Bind(Func<Result> binder);

    public abstract Result<T> Tap(Action<T> action);
    public abstract Result Tap(Action action);

    public abstract Result<T> TapError(Action<string> action);
    public abstract Result TapError(Action action);
    
    public abstract void Deconstruct(out bool isSuccess, out string? errorMessage, out T? result);
    

}