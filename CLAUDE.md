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

There is no test project in this repository yet — `dotnet test` has nothing to run.

## Architecture

This is a single .NET 9 class library (`FunctionalTypes/FunctionalTypes.csproj`, assembly name `FunctionalTypes`, root namespace `FunctionalTypes`) implementing a Railway-Oriented Programming `Result` type in three parallel variants, each in its own folder/namespace:

- `FunctionalTypes.SimpleResult` — non-generic `Result` (success/failure with no payload, `string` error).
- `FunctionalTypes.TypedResult` — generic `Result<T>` (success carries a `T` value, `string` error).
- `FunctionalTypes.TypedErrorResult` — generic `Result<T, TError>` (success carries `T`, failure carries a typed `TError`). Currently only the abstract base class exists here; `Success`/`Failure` implementations have not been added yet.

Each variant follows the same shape: an `abstract class Result[...]` declaring the operations, plus sealed `Success`/`Failure` subclasses implementing them. Core operations across variants:

- `Map` — transform the success value (or produce a new type on success), passing through failures unchanged.
- `Bind` — chain into another `Result`-returning operation (monadic bind); short-circuits on failure.
- `Match` — fold both branches into a single `TR` value.
- `Tap` / `TapError` — side-effect hooks that run on success/failure respectively without altering the result.
- `Check` — validate the success value against a predicate, converting to a `Failure` if it doesn't hold.

The variants are cross-referenced: `SimpleResult.Result` methods can return `TypedResult.Result<TR>` (e.g. `Map`), and `TypedResult.Result<T>` methods can return the non-generic `SimpleResult.Result` (e.g. `Bind(Func<Result> binder)`), so changes to one variant's method signatures often need matching changes in the others to keep the trio consistent. When adding an operation to one `Result` type, check whether the same operation should exist on the other two for API symmetry.

`TypedErrorResult.Result<T, TError>` mirrors this design but is not yet fully implemented — treat it as in-progress and consistent with the finished pattern in `SimpleResult`/`TypedResult` when completing it.
