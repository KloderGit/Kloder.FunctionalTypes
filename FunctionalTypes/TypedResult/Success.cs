using FunctionalTypes.SimpleResult;

namespace FunctionalTypes.TypedResult;

public sealed class Success<T>(T value) : Result<T>
{
    public override Result<TR> Map<TR>(Func<T, TR> selector) => new Success<TR>(selector(value));
    public override Result<TR> Map<TR>(Func<TR> selector) => new Success<TR>(selector());

    public override Result<T> Check(Predicate<T> predicate, string? message = null)
        => predicate(value) ? this : new Failure<T>(message ?? "Check failed");

    public override TR Match<TR>(Func<T, TR> success, Func<string, TR> failure)
        => success(value);

    public override TR Match<TR>(Func<TR> success, Func<string, TR> failure)
        => success();

    public override Result<TR> Bind<TR>(Func<T, Result<TR>> binder) => binder(value);
    public override Result Bind(Func<Result> binder) => binder();

    public override Result<T> Tap(Action<T> action)
    {
        action(value);
        return this;
    }

    public override Result Tap(Action action)
    {
        action();
        return new Success();
    }

    public override Result<T> TapError(Action<string> action)
        => this;

    public override Result TapError(Action action)
        => new Success();

    public override void Deconstruct(out bool isSuccess, out string? errorMessage, out T? result)
    {
        isSuccess = true;
        errorMessage = null;
        result = value;
    }
}