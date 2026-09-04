using FunctionalTypes.Maybe;

namespace UnitTest;

[TestFixture]
public class MaybeApplicativeExtensionsTests
{
    [Test]
    public void Apply_BothJust_CombinesValues()
    {
        Maybe<Func<int, Func<int, string>>> func =
            ApplicativeExtensions.Func<Func<int, Func<int, string>>>(a => b => (a + b).ToString());

        Maybe<Func<int, string>> partial = func.Apply(Maybe<int>.Just(20));
        Maybe<string> result = partial.Apply(Maybe<int>.Just(22));

        Assert.That(result.Match(v => v, () => "nothing"), Is.EqualTo("42"));
    }

    [Test]
    public void Apply_FuncIsNothing_ShortCircuitsToNothing()
    {
        Maybe<Func<int, string>> func = Maybe<Func<int, string>>.Nothing();

        Maybe<string> result = func.Apply(Maybe<int>.Just(42));

        Assert.That(result.IsNothing, Is.True);
    }

    [Test]
    public void Apply_ArgIsNothing_ShortCircuitsToNothing()
    {
        Maybe<Func<int, string>> func = ApplicativeExtensions.Func<Func<int, string>>(v => v.ToString());

        Maybe<string> result = func.Apply(Maybe<int>.Nothing());

        Assert.That(result.IsNothing, Is.True);
    }

    [Test]
    public async Task ApplyAsync_FuncSyncArgAsync_BothJust_CombinesValues()
    {
        Maybe<Func<int, string>> func = ApplicativeExtensions.Func<Func<int, string>>(v => v.ToString());

        Maybe<string> result = await func.ApplyAsync(() => Task.FromResult(Maybe<int>.Just(42)));

        Assert.That(result.Match(v => v, () => "nothing"), Is.EqualTo("42"));
    }

    [Test]
    public async Task ApplyAsync_FuncIsNothing_ShortCircuitsWithoutProducingArg()
    {
        Maybe<Func<int, string>> func = Maybe<Func<int, string>>.Nothing();
        var argFactoryWasCalled = false;

        Maybe<string> result = await func.ApplyAsync(() =>
        {
            argFactoryWasCalled = true;
            return Task.FromResult(Maybe<int>.Just(42));
        });

        Assert.That(argFactoryWasCalled, Is.False);
        Assert.That(result.IsNothing, Is.True);
    }

    [Test]
    public async Task Apply_FuncAsyncArgSync_BothJust_CombinesValues()
    {
        Task<Maybe<Func<int, string>>> funcTask = Task.FromResult(ApplicativeExtensions.Func<Func<int, string>>(v => v.ToString()));

        Maybe<string> result = await funcTask.Apply(Maybe<int>.Just(42));

        Assert.That(result.Match(v => v, () => "nothing"), Is.EqualTo("42"));
    }

    [Test]
    public async Task ApplyAsync_FuncAsyncArgAsync_BothJust_CombinesValues()
    {
        Task<Maybe<Func<int, string>>> funcTask = Task.FromResult(ApplicativeExtensions.Func<Func<int, string>>(v => v.ToString()));

        Maybe<string> result = await funcTask.ApplyAsync(() => Task.FromResult(Maybe<int>.Just(42)));

        Assert.That(result.Match(v => v, () => "nothing"), Is.EqualTo("42"));
    }
}
