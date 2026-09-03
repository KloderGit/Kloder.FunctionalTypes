using FunctionalTypes.TypedResult;

namespace FunctionalTypes.SimpleResult;

public abstract class Result
{
    public abstract Result<TR> Map<TR>(Func<TR> selector);
    public abstract TR Match<TR>(Func<TR> success, Func<string, TR> failure);
    public abstract Result Bind(Func<Result> binder);
    public abstract Result<TR> Bind<TR>(Func<Result<TR>> binder);
    public abstract Result Tap(Action action);
    public abstract Result TapError(Action action);
    public abstract Result Check(Func<bool> predicate, string? message = null);
    
    public abstract void Deconstruct(out bool isSuccess, out string? errorMessage);
}