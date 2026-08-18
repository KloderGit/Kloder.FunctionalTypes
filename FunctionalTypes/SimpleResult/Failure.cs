using FunctionalTypes.TypedResult;

namespace FunctionalTypes.SimpleResult;

public sealed class Failure(string error) : Result
{
    public override Result<TR> Map<TR>(Func<TR> selector)
        => new Failure<TR>(error);

    public override TR Match<TR>(Func<TR> success, Func<string, TR> failure)
        => failure(error);

    public override Result Bind(Func<Result> binder)
        => this;

    public override Result<TR> Bind<TR>(Func<Result<TR>> binder)
        => new Failure<TR>(error);

    public override Result Tap(Action action)
        => this;

    public override Result TapError<TR>(Action action)
    {
        action();
        return this;
    }

    public override Result Check(Func<bool> predicate, string? message = null)
        => this;

    public override void Deconstruct(out bool isSuccess, out string? errorMessage)
    {
        isSuccess = false;
        errorMessage = error;
    }
}