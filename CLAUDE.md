# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

Build the solution:
```
dotnet build KloderGit.FunctionalTypes.sln
```

Restore packages (rarely needed, no external dependencies):
```
dotnet restore KloderGit.FunctionalTypes.sln
```

Run tests (NUnit, in `UnitTest/UnitTest.csproj`):
```
dotnet test KloderGit.FunctionalTypes.sln
```

## Architecture

This is a single .NET 9 class library (`FunctionalTypes/FunctionalTypes.csproj`, assembly name `FunctionalTypes`, root namespace `FunctionalTypes`) implementing Railway-Oriented Programming types, plus a matching `UnitTest` project (NUnit) whose file layout mirrors the source folders.

### The Result trio

A `Result` type in three parallel, fully-implemented variants, each in its own folder/namespace:

- `FunctionalTypes.SimpleResult` — non-generic `Result` (success/failure with no payload, `string` error).
- `FunctionalTypes.TypedResult` — generic `Result<T>` (success carries a `T` value, `string` error).
- `FunctionalTypes.TypedErrorResult` — generic `Result<T, TError>` (success carries `T`, failure carries a typed `TError`).

Each variant follows the same shape: an `abstract class Result[...]` declaring the operations, plus sealed `Success`/`Failure` subclasses implementing them. Core operations across variants:

- `Map` — transform the success value (or produce a new type on success), passing through failures unchanged.
- `Bind` — chain into another `Result`-returning operation (monadic bind); short-circuits on failure.
- `Match` — fold both branches into a single `TR` value.
- `Tap` / `TapError` — side-effect hooks that run on success/failure respectively without altering the result.
- `Check` — validate the success value against a predicate, converting to a `Failure` if it doesn't hold.

The variants are cross-referenced: `SimpleResult.Result` methods can return `TypedResult.Result<TR>` (e.g. `Map`), and `TypedResult.Result<T>` methods can return the non-generic `SimpleResult.Result` (e.g. `Bind(Func<Result> binder)`), so changes to one variant's method signatures often need matching changes in the others to keep the trio consistent. When adding an operation to one `Result` type, check whether the same operation should exist on the other two for API symmetry.

### Extensions layered on top of each variant

Each variant folder also has:

- `ApplicativeExtensions.cs` — `Func`/`Apply`/`ApplyAsync` for applying a `Result`-wrapped function to a `Result`-wrapped argument (`SimpleResult` only has the `Func` lifting helpers, since it carries no value to apply against).
- `BindAsyncExtensions.cs` — async `Bind`, in three shapes: `Result → Task<Result...>`, `Task<Result> → sync binder`, `Task<Result> → Task<Result...>`. Each shape also has cross-variant overloads (an `errorSelector` parameter) that bridge into whichever other variant(s) `Bridging/ResultBridgeExtensions.cs` supports for `Bind`.
- `MapAsyncExtensions.cs` — same three shapes as `BindAsyncExtensions.cs`, for `Map` instead of `Bind`.
- `TapAsyncExtensions.cs` — same three shapes again, for `Tap`/`TapError`. These don't change the result's success/error type, so there's no `Bridging`-based cross-variant overload; the only extra flavor is `TypedResult`'s built-in `Tap(Action)`/`TapError(Action)` bridge down to `SimpleResult.Result` (mirrored in async), which `TypedErrorResult` does **not** have — its `Tap(Action)`/`TapError(Action)` stay `Result<T, TError>`. That asymmetry is pre-existing in the sync API; preserve it rather than "fixing" it.

When adding a sync operation to a `Result` type, also consider whether it needs an async counterpart in the matching `*AsyncExtensions.cs` file, in all three shapes, including the cross-variant bridging overload where `Bridging/ResultBridgeExtensions.cs` has a sync counterpart to mirror.

**Gotcha:** when a `Match` branch is written as an inline `async () => { ... }` lambda (rather than passing an already-typed `Func<...>` variable straight through, the way `BindAsync` passes `binder`), the compiler cannot infer `Match`'s own `TR` type argument from it (`CS0411`/`CS1593`). Give it explicitly, e.g. `result.Match<Task<Result<T>>>(success: async value => ..., failure: ...)`. This shows up whenever the success/failure branch has to *combine* two things (await a side effect or a plain value, then wrap/return a `Result`) instead of directly returning an already-`Result`-shaped delegate.

### Bridging between variants

`FunctionalTypes.Bridging.ResultBridgeExtensions` (`Bridging/ResultBridgeExtensions.cs`) holds synchronous `Map`/`Bind` extensions that cross from one variant into another (e.g. `SimpleResult.Result.Bind` into `Result<TR, TError>`). Coverage is not fully symmetric by design: `TypedErrorResult → SimpleResult` only has `Bind`, not `Map`, since `SimpleResult` carries no value for a `Map` selector to produce — that asymmetry is intentional and should be preserved (and mirrored the same way in the async extensions) rather than "completed".

### Either

`FunctionalTypes.Either` (`Either/Either.cs`, `Left.cs`, `Right.cs`) is a separate, independent `Either<TLeft, TRight>` type — not part of the `Result` trio's cross-referencing. It has `MapLeft`/`MapRight`, `BindLeft`/`BindRight`, `Match`, `TapLeft`/`TapRight`, `Swap`, `Deconstruct`. `Either/BindExtensions.cs` bridges it into `TypedResult.Result<T>` pipelines (`Bind` with separate left/right continuations, and `Collapse` when both sides carry the same type). Either currently has no async counterpart (no `BindAsync`/`MapAsync`) — unlike the `Result` trio, which has async extensions throughout.
