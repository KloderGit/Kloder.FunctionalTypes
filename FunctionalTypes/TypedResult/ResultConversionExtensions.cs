namespace FunctionalTypes.TypedResult;

public static class ResultConversionExtensions
{
    public static Result<T> ToResult<T>(this T value) => new Success<T>(value);
}
