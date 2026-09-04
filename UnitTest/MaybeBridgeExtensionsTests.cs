using FunctionalTypes.Bridging;
using FunctionalTypes.Maybe;
using FunctionalTypes.TypedErrorResult;
using FunctionalTypes.TypedResult;

namespace UnitTest;

[TestFixture]
public class MaybeBridge_SyncTests
{
    [Test]
    public void ToResult_TypedResult_OnJust_ProducesSuccess()
    {
        Maybe<int> maybe = Maybe<int>.Just(42);

        Result<int> result = maybe.ToResult("was nothing");

        Assert.That(result.Match(v => v, e => -1), Is.EqualTo(42));
    }

    [Test]
    public void ToResult_TypedResult_OnNothing_ProducesFailureWithMessage()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();

        Result<int> result = maybe.ToResult("was nothing");

        Assert.That(result.Match(_ => "success", e => e), Is.EqualTo("was nothing"));
    }

    [Test]
    public void ToResult_TypedErrorResult_OnJust_ProducesSuccess()
    {
        Maybe<int> maybe = Maybe<int>.Just(42);

        Result<int, DomainError> result = maybe.ToResult(() => DomainError.NotFound);

        Assert.That(result.Match(v => v, e => -1), Is.EqualTo(42));
    }

    [Test]
    public void ToResult_TypedErrorResult_OnNothing_ProducesFailureFromFactory()
    {
        Maybe<int> maybe = Maybe<int>.Nothing();

        Result<int, DomainError> result = maybe.ToResult(() => DomainError.NotFound);

        Assert.That(result.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }

    [Test]
    public void ToMaybe_FromTypedResult_OnSuccess_ProducesJust()
    {
        Result<int> result = new Success<int>(42);

        Maybe<int> maybe = result.ToMaybe();

        Assert.That(maybe.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public void ToMaybe_FromTypedResult_OnFailure_ProducesNothing()
    {
        Result<int> result = new Failure<int>("boom");

        Maybe<int> maybe = result.ToMaybe();

        Assert.That(maybe.IsNothing, Is.True);
    }

    [Test]
    public void ToMaybe_FromTypedErrorResult_OnSuccess_ProducesJust()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);

        Maybe<int> maybe = result.ToMaybe();

        Assert.That(maybe.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public void ToMaybe_FromTypedErrorResult_OnFailure_ProducesNothing()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);

        Maybe<int> maybe = result.ToMaybe();

        Assert.That(maybe.IsNothing, Is.True);
    }
}

[TestFixture]
public class MaybeBridge_AsyncTests
{
    [Test]
    public async Task ToResult_TypedResult_OnTaskJust_ProducesSuccess()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Just(42));

        Result<int> result = await maybeTask.ToResult("was nothing");

        Assert.That(result.Match(v => v, e => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task ToResult_TypedErrorResult_OnTaskNothing_ProducesFailureFromFactory()
    {
        Task<Maybe<int>> maybeTask = Task.FromResult(Maybe<int>.Nothing());

        Result<int, DomainError> result = await maybeTask.ToResult(() => DomainError.NotFound);

        Assert.That(result.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }

    [Test]
    public async Task ToMaybe_FromTaskTypedResult_OnSuccess_ProducesJust()
    {
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(new Success<int>(42));

        Maybe<int> maybe = await resultTask.ToMaybe();

        Assert.That(maybe.Match(v => v, () => -1), Is.EqualTo(42));
    }

    [Test]
    public async Task ToMaybe_FromTaskTypedErrorResult_OnFailure_ProducesNothing()
    {
        Task<Result<int, DomainError>> resultTask =
            Task.FromResult<Result<int, DomainError>>(new Failure<int, DomainError>(DomainError.NotFound));

        Maybe<int> maybe = await resultTask.ToMaybe();

        Assert.That(maybe.IsNothing, Is.True);
    }
}
