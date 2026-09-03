using FunctionalTypes.SimpleResult;

namespace FunctionalTypes.TypedResult;

public static class BoolExtensions
{
    // factory is only invoked when isValid is true
    public static Result<TR> Then<TR>(this bool isValid, Func<TR> factory, string error) =>
        isValid ? new Success<TR>(factory()) : new Failure<TR>(error);

    public static Result ToResult(this bool isValid, string error) =>
        isValid ? new Success() : new Failure(error);
}
