using FunctionalTypes.TypedResult;

namespace FunctionalTypes.SimpleResult;

public sealed class Success : Result
{
    public override Result<TR> Map<TR>(Func<TR> selector)
        => new Success<TR>(selector());

    public override TR Match<TR>(Func<TR> success, Func<string, TR> failure)
        => success();

    public override Result Bind(Func<Result> binder)
        => binder();

    public override Result<TR> Bind<TR>(Func<Result<TR>> binder)
        => binder();

    public override Result Tap(Action action)
        { action(); return this; }

    public override Result TapError(Action action)
        { return this; }

    public override Result Check(Func<bool> predicate, string? message = null)
        => predicate() ? this : new Failure(message ?? "Check failed");

    public override void Deconstruct(out bool isSuccess, out string? errorMessage)
        { isSuccess = true; errorMessage = null; }
}