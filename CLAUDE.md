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

## Architecture.

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
- `CheckAsyncExtensions.cs` — same three shapes for `Check`, with no bridging variant (the sync `Check` never bridges across variants either). The predicate becomes async (`Func<Task<bool>>` / `Func<T, Task<bool>>`); the failure-side parameter (`message` / `errorFactory`) stays sync, matching how `errorSelector` stays sync in `BindAsyncExtensions.cs`/`MapAsyncExtensions.cs` — only the "main" async operation gets awaited, error/message production does not.
- `MatchAsyncExtensions.cs` — same three shapes for `Match`. Unlike `Tap`/`Map`/`Check`, `success`/`failure` here don't need to be combined with anything else before returning — they already *are* the `TR` the sync `Match<TR>` needs, just with `TR = Task<...>`. So they can be forwarded straight into the existing sync `Match` call (`result.Match(success, failure)`), exactly like `BindAsync` forwards `binder` directly — no inline `async () => {...}` wrapper, and consequently no explicit `Match<Task<TR>>(...)` type argument needed (that gotcha above is specific to methods that must combine two things into one delegate).

When adding a sync operation to a `Result` type, also consider whether it needs an async counterpart in the matching `*AsyncExtensions.cs` file, in all three shapes, including the cross-variant bridging overload where `Bridging/ResultBridgeExtensions.cs` has a sync counterpart to mirror.

**Gotcha:** when a `Match` branch is written as an inline `async () => { ... }` lambda (rather than passing an already-typed `Func<...>` variable straight through, the way `BindAsync` passes `binder`), the compiler cannot infer `Match`'s own `TR` type argument from it (`CS0411`/`CS1593`). Give it explicitly, e.g. `result.Match<Task<Result<T>>>(success: async value => ..., failure: ...)`. This shows up whenever the success/failure branch has to *combine* two things (await a side effect or a plain value, then wrap/return a `Result`) instead of directly returning an already-`Result`-shaped delegate.

### Bridging between variants

`FunctionalTypes.Bridging.ResultBridgeExtensions` (`Bridging/ResultBridgeExtensions.cs`) holds synchronous `Map`/`Bind` extensions that cross from one variant into another (e.g. `SimpleResult.Result.Bind` into `Result<TR, TError>`). Coverage is not fully symmetric by design: `TypedErrorResult → SimpleResult` only has `Bind`, not `Map`, since `SimpleResult` carries no value for a `Map` selector to produce — that asymmetry is intentional and should be preserved (and mirrored the same way in the async extensions) rather than "completed".

### Either

`FunctionalTypes.Either` (`Either/Either.cs`, `Left.cs`, `Right.cs`) is a separate, independent `Either<TLeft, TRight>` type — not part of the `Result` trio's cross-referencing. It has `MapLeft`/`MapRight`, `BindLeft`/`BindRight`, `Match`, `TapLeft`/`TapRight`, `Swap`, `Deconstruct`. `Either/BindExtensions.cs` bridges it into `TypedResult.Result<T>` pipelines (`Bind` with separate left/right continuations, and `Collapse` when both sides carry the same type).

Either has full async coverage now, mirroring the `Result` trio's `*AsyncExtensions.cs` pattern, one file per operation family in the `Either` folder:

- `MapAsyncExtensions.cs` — `MapLeftAsync`/`MapRightAsync`, three shapes, own type only (no bridging target for `Map`).
- `BindAsyncExtensions.cs` — `BindLeftAsync`/`BindRightAsync` (own type, three shapes) **plus** the async counterpart of `BindExtensions.cs`'s bridge into `Result<TR>` (`BindAsync`/`Bind`/`BindAsync` on `Result<Either<TLeft, TRight>>` / `Task<Result<Either<TLeft, TRight>>>`). The bridge's async implementation is a one-liner because it just forwards into `TypedResult.BindAsyncExtensions.BindAsync`, which already exists.
- `TapAsyncExtensions.cs` — `TapLeftAsync`/`TapRightAsync`, three shapes.
- `MatchAsyncExtensions.cs` — `MatchAsync`, three shapes; like `Result`'s `MatchAsync`, `onLeft`/`onRight` forward straight into the sync `Match` (no inline-lambda wrapper, no explicit `Match<TR>` type argument needed).

No `CollapseAsync` was added: `Collapse` is literally `result.Map(either => either.Match(x => x, x => x))`, so it already gets async support for free from `TypedResult.MapAsyncExtensions`'s existing `Map`/`MapAsync` on `Task<Result<T>>` — adding a dedicated method would just duplicate that. `Swap` also has no async counterpart, for the same reason `Deconstruct` doesn't on `Result`: it takes no delegate to await, so there's nothing an async version would add over `(await task).Swap()`.

### Maybe

`FunctionalTypes.Maybe` (`Maybe/Maybe.cs`, `Just.cs`, `Nothing.cs`) is an `Option`-style `Maybe<T>` — a third independent type alongside the `Result` trio and `Either`, following the exact same shape (abstract class + `Just`/`Nothing` sealed subclasses, static `Maybe<T>.Just(value)`/`Maybe<T>.Nothing()` factories). Its operations mirror `Result`'s, minus anything that would need an error payload `Nothing` doesn't have:

- `Map`, `Bind`, `Match(Func<T,TR> just, Func<TR> nothing)`, `Tap`/`TapNothing` (the `Nothing`-side counterpart of `TapError`, but parameterless — `Nothing` carries nothing to pass it), `Check(Predicate<T>)` (fails → becomes `Nothing`, no message), `Deconstruct(out bool hasValue, out T? value)`.
- `GetValueOrDefault(T defaultValue)` — the one operation `Result` doesn't have. `Result` never got an equivalent because `Match(v => v, () => default)` already does the job there and nobody asked for the shortcut; `Maybe` gets it because pulling a plain value out of an `Option` with a fallback is a near-universal ask for that shape of type.

Same extension-file layout as `Result`/`Either`, in the `Maybe` folder: `ApplicativeExtensions.cs`, `BindAsyncExtensions.cs`, `MapAsyncExtensions.cs`, `TapAsyncExtensions.cs`, `CheckAsyncExtensions.cs`, `MatchAsyncExtensions.cs` — same three shapes, same "pass the delegate straight through when it already returns the right shape, wrap in an inline lambda (with explicit `Match<TR>(...)` argument) when it doesn't" rule as `Result`/`Either`.

`Bridging/MaybeBridgeExtensions.cs` (sync) and `Bridging/MaybeBridgeAsyncExtensions.cs` (`Task`-forwarding) connect `Maybe<T>` to the `Result` trio: `Maybe<T>.ToResult(string noneMessage)` → `TypedResult.Result<T>`, `Maybe<T>.ToResult<TError>(Func<TError> errorFactory)` → `TypedErrorResult.Result<T, TError>`, and `ToMaybe()` the other way from both (the error message/value is discarded — `Maybe` has nowhere to put it, same reasoning as `TypedErrorResult → SimpleResult`'s `Map` gap). There's no `Maybe ↔ SimpleResult` bridge, for the same reason `TypedErrorResult → SimpleResult` has no `Map`: `SimpleResult` carries no value to receive `Maybe`'s payload. The async bridge file only has the `Task`-forwarding shape (no true "async" shape) because none of the four methods take a delegate to await — same reasoning as `Swap`/`Deconstruct` having no async counterpart.

**Entering `Maybe` from plain .NET values** — three files, one `ToMaybe()` per source shape:

- `MaybeReferenceExtensions.cs` — `ToMaybe<T>(this T? value) where T : class`: `null` → `Nothing`, else `Just`.
- `MaybeValueExtensions.cs` — `ToMaybe<T>(this T value) where T : struct` (always `Just`) and `ToMaybe<T>(this T? value) where T : struct` (`Nullable<T>`: `null` → `Nothing`, else `Just`).
- `MaybeSequenceExtensions.cs` — `ToMaybe<T>(this IEnumerable<T>? value)`: `null` *or empty* → `Nothing`, else `Just` of the sequence materialized once (reuses it as-is if it's already an `IReadOnlyCollection<T>`, otherwise `.ToList()`s it — never enumerates a lazy sequence twice).

**Gotcha (real, not hypothetical — caught a test failure while building this):** `MaybeSequenceExtensions.ToMaybe` only wins overload resolution when the argument's *static* type is `IEnumerable<T>` itself. When a variable is statically typed as a concrete reference type instead — `List<T>`, `T[]`, etc. — `MaybeReferenceExtensions.ToMaybe<T> where T : class` wins instead, because C# always prefers an identity-match candidate (`T = List<int>` needs no conversion) over one needing an interface conversion (`IEnumerable<T>` from `List<int>` does), regardless of which is "more specific" generically. That reference overload only null-checks; it has no idea about "empty means `Nothing`". So `new List<int>().ToMaybe()` silently returns `Just(emptyList)`, not `Nothing` — the empty-check only fires when the variable is declared/typed as `IEnumerable<T>` (e.g. it came back from a method whose return type is `IEnumerable<T>`), not when it's a concrete collection type. This was a deliberate, discussed trade-off (see `MaybeSequenceExtensions.ToMaybe`'s XML doc) — not a bug to "fix" by renaming the method; `null → Nothing` still works correctly either way, only the empty-check is affected.

### Entering `Result` from plain values

Mirrors the `Maybe` conversion extensions above, for `Result`/`Result<T>`:

- `TypedResult/ResultConversionExtensions.cs` — `T.ToResult()`: unconstrained, always `Success<T>` (there's no error message to invent for "T was somehow invalid", so no null-check — unlike `Maybe`'s reference overload).
- `TypedResult/BoolExtensions.cs` — `bool.Then<TR>(Func<TR> factory, string error)` → `Result<TR>` (`factory` only runs when `true`) and `bool.ToResult(string error)` → `SimpleResult.Result`. Lives in `TypedResult` (not `Bridging`) because `bool` isn't one of this library's own type families — these are "enter `Result` from a plain condition" helpers, the same spirit as `Maybe`'s `ToMaybe()` set, not a bridge between two internal types.

### `Traverse` / `Sequence`

`TraverseExtensions.cs` (sync) and `TraverseAsyncExtensions.cs` (async) exist in `TypedResult`, `TypedErrorResult`, and `Maybe` (not `SimpleResult` or `Either` — nobody's asked for those yet). `Traverse<T,TR>(this IEnumerable<T> source, Func<T, Result<TR>> selector)` applies `selector` to each item and short-circuits with the *first* failure/`Nothing` (remaining items are never even passed to `selector`) — otherwise collects every value into one `Success`/`Just` of `IEnumerable<TR>` (already fully materialized as a `List<TR>` under the interface, same "typed as the interface, not lazy" shape as `Maybe`'s `ToMaybe()` on sequences). `Sequence<T>(this IEnumerable<Result<T>> source)` is the `T`-identity special case (`source.Traverse(x => x)`), for when the items are already wrapped.

Implemented via `Deconstruct`, not `is Failure<T>` pattern-matching — the whole library treats `Match`/`Deconstruct` as the one sanctioned way to inspect a `Result`/`Maybe`; reaching for `is` on a concrete subclass bypasses that and was flagged as a style problem in an early draft of this operation.

Async surface is deliberately **two** methods, not the usual three (`X → Task`, `Task → sync`, `Task → Task`) shapes used everywhere else in this file: `TraverseAsync` (selector is `Func<T, Task<Result<TR>>>`, awaited sequentially, short-circuiting the same way) and `SequenceAsync` (over `IEnumerable<Task<Result<T>>>` — already-started tasks, awaited one by one). There's no third "`Task<IEnumerable<T>>` as input" shape because it wouldn't add anything: unlike a single `Result`, which is often itself the product of an async computation, an `IEnumerable<T>` source is normally already in hand by the time you're ready to traverse it — and if it isn't, `await`-ing it first is a one-liner that doesn't need library support.

**Gotcha when writing tests (or any call site) against `Traverse`/`Sequence`:** a lambda/array mixing `new Success<T>(...)` and `new Failure<T>(...)` branches (e.g. `v == 2 ? new Failure<int>(msg) : new Success<int>(v)`, or `new[] { new Success<int>(1), new Failure<int>("x") }`) fails to compile — `CS0411`/`CS0826`/`CS1662` — because `Success<T>` and `Failure<T>` have no common type on their own; the compiler needs to be told the target is `Result<T>` (cast one branch, or declare the array as `Result<T>[]`) before it'll unify them. `Maybe<T>.Just(...)`/`.Nothing()` don't have this problem because those are *static factory methods already typed to return `Maybe<T>`*, unlike `Result`/`Result<T,TError>`, which have no equivalent factory and are only ever constructed via the concrete `Success`/`Failure` subclasses directly.
