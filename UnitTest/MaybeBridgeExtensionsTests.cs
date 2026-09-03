using FunctionalTypes.Bridging;
using FunctionalTypes.Maybe;
using FunctionalTypes.TypedErrorResult;
using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class MaybeBridge_SyncTests
{
    [Test]
    public void ToResult_TypedResult_OnSome_ProducesSuccess()
    {
        Maybe<int> maybe = Maybe<int>.Some(42);

        Result<int> result = maybe.ToResult("was none");

        Assert.That(result.Match(v => v, e => -1), Is.EqualTo(42));
    }

    [Test]
    public void ToResult_TypedResult_OnNone_ProducesFailureWithMessage()
    {
        Maybe<int> maybe = Maybe<int>.None();

        Result<int> result = maybe.ToResult("was none");

        Assert.That(result.Match(_ => "success", e => e), Is.EqualTo("was none"));
    }

    [Test]
    public void ToResult_TypedErrorResult_OnSome_ProducesSuccess()
    {
        Maybe<int> maybe = Maybe<int>.Some(42);

        Result<int, DomainError> result = maybe.ToResult(() => DomainError.NotFound);

        Assert.That(result.Match(v => v, e => -1), Is.EqualTo(42));
    }

    [Test]
    public void ToResult_TypedErrorResult_OnNone_ProducesFailureFromFactory()
    {
        Maybe<int> maybe = Maybe<int>.None();

        Result<int, DomainError> result = maybe.ToResult(() => DomainError.NotFound);

        Assert.That(result.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }

    [Test]
    public void ToMaybe_FromTypedResult_OnSuccess_ProducesSome()
    {
        Result<int> result = new Success<int>(42);

        Maybe<int> maybe = result.ToMaybe();

        Assert.That(maybe.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public void ToMaybe_FromTypedResult_OnFailure_ProducesNone()
    {
        Result<int> result = new Failure<int>("boom");

        Maybe<int> maybe = result.ToMaybe();

        Assert.That(maybe.IsNone, Is.True);
    }

    [Test]
    public void ToMaybe_FromTypedErrorResult_OnSuccess_ProducesSome()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);

        Maybe<int> maybe = result.ToMaybe();

        Assert.That(maybe.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public void ToMaybe_FromTypedErrorResult_OnFailure_ProducesNone()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);

        Maybe<int> maybe = result.ToMaybe();

        Assert.That(maybe.IsNone, Is.True);
    }
}

[TestFixture]
public class MaybeBridge_AsyncTests
{
    [Test]
    public async Task ToResult_TypedResult_OnTaskSome_ProducesSuccess()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Some(42));

        Result<int> result = await maybeTask.ToResult("was none");

        Assert.That(result.Match(v => v, e => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task ToResult_TypedErrorResult_OnTaskNone_ProducesFailureFromFactory()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.None());

        Result<int, DomainError> result = await maybeTask.ToResult(() => DomainError.NotFound);

        Assert.That(result.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }

    [Test]
    public async Task ToMaybe_FromTaskTypedResult_OnSuccess_ProducesSome()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));

        Maybe<int> maybe = await resultTask.ToMaybe();

        Assert.That(maybe.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task ToMaybe_FromTaskTypedErrorResult_OnFailure_ProducesNone()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));

        Maybe<int> maybe = await resultTask.ToMaybe();

        Assert.That(maybe.IsNone, Is.True);
    }
}
