using FunctionalTypes.Bridging;
using FunctionalTypes.SimpleResult;
using FunctionalTypes.TypedErrorResult;
using FunctionalTypes.TypedResult;

namespace UnitTest;

public enum DomainError
{
    NotFound,
    Invalid
}

[TestFixture]
public class SimpleToTypedTransitionTests
{
    [Test]
    public void Map_OnSuccess_ProducesTypedSuccess()
    {
        Result result = new Success();

        Result<int> mapped = result.Map(() => 42);

        Assert.That(mapped.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public void Map_OnFailure_ProducesTypedFailureWithSameMessage()
    {
        Result result = new Failure("boom");

        Result<int> mapped = result.Map(() => 42);

        Assert.That(mapped.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public void Bind_OnSuccess_RunsBinder()
    {
        Result result = new Success();

        Result<int> bound = result.Bind(() => new Success<int>(7));

        Assert.That(bound.Match(v => v, _ => -1), Is.EqualTo(7));
    }

    [Test]
    public void Bind_OnFailure_ShortCircuitsWithoutRunningBinder()
    {
        Result result = new Failure("boom");
        var binderWasCalled = false;

        Result<int> bound = result.Bind<int>(() =>
        {
            binderWasCalled = true;
            return new Success<int>(7);
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => "success", e => e), Is.EqualTo("boom"));
    }
}

[TestFixture]
public class TypedToSimpleTransitionTests
{
    [Test]
    public void Bind_OnSuccess_RunsBinder()
    {
        Result<int> result = new Success<int>(42);

        Result bound = result.Bind(() => new Success());

        Assert.That(bound.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public void Bind_OnFailure_ShortCircuitsWithoutRunningBinder()
    {
        Result<int> result = new Failure<int>("boom");
        var binderWasCalled = false;

        Result bound = result.Bind(() =>
        {
            binderWasCalled = true;
            return new Success();
        });

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(() => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public void Tap_OnSuccess_RunsActionAndProducesSimpleSuccess()
    {
        Result<int> result = new Success<int>(42);
        var tapWasCalled = false;

        Result tapped = result.Tap(() => tapWasCalled = true);

        Assert.That(tapWasCalled, Is.True);
        Assert.That(tapped.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public void Tap_OnFailure_SkipsActionAndProducesSimpleFailureWithSameMessage()
    {
        Result<int> result = new Failure<int>("boom");
        var tapWasCalled = false;

        Result tapped = result.Tap(() => tapWasCalled = true);

        Assert.That(tapWasCalled, Is.False);
        Assert.That(tapped.Match(() => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public void TapError_OnFailure_RunsActionAndProducesSimpleFailureWithSameMessage()
    {
        Result<int> result = new Failure<int>("boom");
        var tapErrorWasCalled = false;

        Result tapped = result.TapError(() => tapErrorWasCalled = true);

        Assert.That(tapErrorWasCalled, Is.True);
        Assert.That(tapped.Match(() => "success", e => e), Is.EqualTo("boom"));
    }

    [Test]
    public void TapError_OnSuccess_SkipsActionAndProducesSimpleSuccess()
    {
        Result<int> result = new Success<int>(42);
        var tapErrorWasCalled = false;

        Result tapped = result.TapError(() => tapErrorWasCalled = true);

        Assert.That(tapErrorWasCalled, Is.False);
        Assert.That(tapped.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }
}

[TestFixture]
public class SimpleToTypedErrorTransitionTests
{
    [Test]
    public void Map_OnSuccess_ProducesTypedErrorSuccess()
    {
        Result result = new Success();

        Result<int, DomainError> mapped = result.Map<int, DomainError>(
            () => 42,
            errorSelector: _ => DomainError.Invalid);

        Assert.That(mapped.Match(v => v, _ => -1), Is.EqualTo(42));
    }

    [Test]
    public void Map_OnFailure_ConvertsStringErrorViaSelector()
    {
        Result result = new Failure("not-found");

        Result<int, DomainError> mapped = result.Map<int, DomainError>(
            () => 42,
            errorSelector: msg => msg == "not-found" ? DomainError.NotFound : DomainError.Invalid);

        Assert.That(mapped.Match(_ => DomainError.Invalid, e => e), Is.EqualTo(DomainError.NotFound));
    }

    [Test]
    public void Bind_OnSuccess_RunsBinder()
    {
        Result result = new Success();

        Result<int, DomainError> bound = result.Bind(
            () => new Success<int, DomainError>(7),
            errorSelector: _ => DomainError.Invalid);

        Assert.That(bound.Match(v => v, _ => -1), Is.EqualTo(7));
    }

    [Test]
    public void Bind_OnFailure_ShortCircuitsAndConvertsError()
    {
        Result result = new Failure("boom");
        var binderWasCalled = false;

        Result<int, DomainError> bound = result.Bind<int, DomainError>(
            () =>
            {
                binderWasCalled = true;
                return new Success<int, DomainError>(7);
            },
            errorSelector: _ => DomainError.Invalid);

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}

[TestFixture]
public class TypedToTypedErrorTransitionTests
{
    [Test]
    public void Map_OnSuccess_ProducesTypedErrorSuccess()
    {
        Result<int> result = new Success<int>(42);

        Result<string, DomainError> mapped = result.Map<int, string, DomainError>(
            v => $"value={v}",
            errorSelector: _ => DomainError.Invalid);

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public void Map_OnFailure_ConvertsStringErrorViaSelector()
    {
        Result<int> result = new Failure<int>("boom");

        Result<string, DomainError> mapped = result.Map<int, string, DomainError>(
            v => $"value={v}",
            errorSelector: _ => DomainError.Invalid);

        Assert.That(mapped.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }

    [Test]
    public void Bind_OnSuccess_RunsBinder()
    {
        Result<int> result = new Success<int>(42);

        Result<string, DomainError> bound = result.Bind<int, string, DomainError>(
            v => new Success<string, DomainError>($"value={v}"),
            errorSelector: _ => DomainError.Invalid);

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public void Bind_OnFailure_ShortCircuitsAndConvertsError()
    {
        Result<int> result = new Failure<int>("boom");
        var binderWasCalled = false;

        Result<string, DomainError> bound = result.Bind<int, string, DomainError>(
            v =>
            {
                binderWasCalled = true;
                return new Success<string, DomainError>($"value={v}");
            },
            errorSelector: _ => DomainError.Invalid);

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
    }
}

[TestFixture]
public class TypedErrorToSimpleTransitionTests
{
    [Test]
    public void Bind_OnSuccess_RunsBinder()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);

        Result bound = result.Bind(
            _ => new Success(),
            errorSelector: e => e.ToString());

        Assert.That(bound.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public void Bind_OnFailure_ShortCircuitsAndConvertsError()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);
        var binderWasCalled = false;

        Result bound = result.Bind(
            _ =>
            {
                binderWasCalled = true;
                return new Success();
            },
            errorSelector: e => e.ToString());

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(() => "success", e => e), Is.EqualTo(nameof(DomainError.NotFound)));
    }
}

[TestFixture]
public class TypedErrorToTypedTransitionTests
{
    [Test]
    public void Map_OnSuccess_ProducesTypedSuccess()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);

        Result<string> mapped = result.Map<int, string, DomainError>(
            v => $"value={v}",
            errorSelector: e => e.ToString());

        Assert.That(mapped.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public void Map_OnFailure_ConvertsErrorViaSelector()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.NotFound);

        Result<string> mapped = result.Map<int, string, DomainError>(
            v => $"value={v}",
            errorSelector: e => e.ToString());

        Assert.That(mapped.Match(_ => "success", e => e), Is.EqualTo(nameof(DomainError.NotFound)));
    }

    [Test]
    public void Bind_OnSuccess_RunsBinder()
    {
        Result<int, DomainError> result = new Success<int, DomainError>(42);

        Result<string> bound = result.Bind<int, string, DomainError>(
            v => new Success<string>($"value={v}"),
            errorSelector: e => e.ToString());

        Assert.That(bound.Match(v => v, _ => "err"), Is.EqualTo("value=42"));
    }

    [Test]
    public void Bind_OnFailure_ShortCircuitsAndConvertsError()
    {
        Result<int, DomainError> result = new Failure<int, DomainError>(DomainError.Invalid);
        var binderWasCalled = false;

        Result<string> bound = result.Bind<int, string, DomainError>(
            v =>
            {
                binderWasCalled = true;
                return new Success<string>($"value={v}");
            },
            errorSelector: e => e.ToString());

        Assert.That(binderWasCalled, Is.False);
        Assert.That(bound.Match(_ => "success", e => e), Is.EqualTo(nameof(DomainError.Invalid)));
    }
}

[TestFixture]
public class EndToEndFlowTests
{
    [Test]
    public void SuccessPath_FlowsThroughAllThreeHierarchiesAndBack()
    {
        Result start = new Success();

        Result<int> typed = start.Bind(() => new Success<int>(42));

        Result<string, DomainError> typedError = typed.Bind<int, string, DomainError>(
            v => new Success<string, DomainError>($"value={v}"),
            errorSelector: _ => DomainError.Invalid);

        Result backToSimple = typedError.Bind(
            _ => new Success(),
            errorSelector: e => e.ToString());

        Assert.That(backToSimple.Match(() => "ok", e => e), Is.EqualTo("ok"));
    }

    [Test]
    public void FailurePath_ShortCircuitsThroughAllThreeHierarchies()
    {
        Result start = new Failure("boom");

        Result<int> typed = start.Bind(() => new Success<int>(1));

        Result<string, DomainError> typedError = typed.Bind<int, string, DomainError>(
            v => new Success<string, DomainError>("unreachable"),
            errorSelector: msg => msg == "boom" ? DomainError.Invalid : DomainError.NotFound);

        Result backToSimple = typedError.Bind(
            _ => new Success(),
            errorSelector: e => e.ToString());

        Assert.That(typedError.Match(_ => DomainError.NotFound, e => e), Is.EqualTo(DomainError.Invalid));
        Assert.That(backToSimple.Match(() => "success", e => e), Is.EqualTo(nameof(DomainError.Invalid)));
    }
}
