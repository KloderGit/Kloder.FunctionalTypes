using FunctionalTypes.SimpleResult;

namespace FunctionalTypes.TypedResult;

public sealed class Failure<T>(string message) : Result<T>
{
    public override Result<TR> Map<TR>(Func<T, TR> selector) => new Failure<TR>(message);
    public override Result<TR> Map<TR>(Func<TR> selector) => new Failure<TR>(message);

    public override Result<T> Check(Predicate<T> predicate, string? message = null) => this;

    public override TR Match<TR>(Func<T, TR> success, Func<string, TR> failure) => failure(message);
    public override TR Match<TR>(Func<TR> success, Func<string, TR> failure) => failure(message);

    public override Result<TR> Bind<TR>(Func<T, Result<TR>> binder) => new Failure<TR>(message);
    public override Result Bind(Func<Result> binder) => new Failure(message);

    public override Result<T> Tap(Action<T> action) => this;
    public override Result Tap(Action action) => new Failure(message);

    public override Result<T> TapError(Action<string> action)
    {
        action(message);
        return this;
    }

    public override Result TapError(Action action)
    {
        action();
        return new Failure(message);
    }

    public override void Deconstruct(out bool isSuccess, out string? errorMessage, out T? result)
    {
        isSuccess = false;
        errorMessage = message;
        result = default;
    }
}