using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeApplicativeExtensionsTests
{
    [Test]
    public void Apply_BothSome_CombinesValues()
    {
        Maybe<Func<int, Func<int, string>>> func =
            ApplicativeExtensions.Func<Func<int, Func<int, string>>>(a => b => (a + b).ToString());

        Maybe<Func<int, string>> partial = func.Apply(Maybe<int>.Some(20));
        Maybe<string> result = partial.Apply(Maybe<int>.Some(22));

        Assert.That(result.Match(v => v, () => "none"), Is.EqualTo("42"));
    }

    [Test]
    public void Apply_FuncIsNone_ShortCircuitsToNone()
    {
        Maybe<Func<int, string>> func = Maybe<Func<int, string>>.None();

        Maybe<string> result = func.Apply(Maybe<int>.Some(42));

        Assert.That(result.IsNone, Is.True);
    }

    [Test]
    public void Apply_ArgIsNone_ShortCircuitsToNone()
    {
        Maybe<Func<int, string>> func = ApplicativeExtensions.Func<Func<int, string>>(v => v.ToString());

        Maybe<string> result = func.Apply(Maybe<int>.None());

        Assert.That(result.IsNone, Is.True);
    }

    [Test]
    public async Task ApplyAsync_FuncSyncArgAsync_BothSome_CombinesValues()
    {
        Maybe<Func<int, string>> func = ApplicativeExtensions.Func<Func<int, string>>(v => v.ToString());

        Maybe<string> result = await func.ApplyAsync(() => Task.FromResult(Maybe<int>.Some(42)));

        Assert.That(result.Match(v => v, () => "none"), Is.EqualTo("42"));
    }

    [Test]
    public async Task ApplyAsync_FuncIsNone_ShortCircuitsWithoutProducingArg()
    {
        Maybe<Func<int, string>> func = Maybe<Func<int, string>>.None();
        var argFactoryWasCalled = false;

        Maybe<string> result = await func.ApplyAsync(() =>
        {
            argFactoryWasCalled = true;
            return Task.FromResult(Maybe<int>.Some(42));
        });

        Assert.That(argFactoryWasCalled, Is.False);
        Assert.That(result.IsNone, Is.True);
    }

    [Test]
    public async Task Apply_FuncAsyncArgSync_BothSome_CombinesValues()
    {
        Task<Maybe<Func<int, string>>> funcTask = Task.FromResult(ApplicativeExtensions.Func<Func<int, string>>(v => v.ToString()));

        Maybe<string> result = await funcTask.Apply(Maybe<int>.Some(42));

        Assert.That(result.Match(v => v, () => "none"), Is.EqualTo("42"));
    }

    [Test]
    public async Task ApplyAsync_FuncAsyncArgAsync_BothSome_CombinesValues()
    {
        Task<Maybe<Func<int, string>>> funcTask = Task.FromResult(ApplicativeExtensions.Func<Func<int, string>>(v => v.ToString()));

        Maybe<string> result = await funcTask.ApplyAsync(() => Task.FromResult(Maybe<int>.Some(42)));

        Assert.That(result.Match(v => v, () => "none"), Is.EqualTo("42"));
    }
}
