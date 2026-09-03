using FunctionalTypes.Maybe;

namespace FunctionalTypes.Bridging;

public static class MaybeBridgeAsyncExtensions
{
    public static async Task<TypedResult.Result<T>> ToResult<T>(this Task<Maybe<T>> maybeTask, string noneMessage) =>
        (await maybeTask).ToResult(noneMessage);

    public static async Task<TypedErrorResult.Result<T, TError>> ToResult<T, TError>(this Task<Maybe<T>> maybeTask,
        Func<TError> errorFactory) =>
        (await maybeTask).ToResult(errorFactory);

    public static async Task<Maybe<T>> ToMaybe<T>(this Task<TypedResult.Result<T>> resultTask) =>
        (await resultTask).ToMaybe();

    public static async Task<Maybe<T>> ToMaybe<T, TError>(this Task<TypedErrorResult.Result<T, TError>> resultTask) =>
        (await resultTask).ToMaybe();
}
