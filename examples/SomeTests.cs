using System;
using NUnit.Framework;

[TestFixture]
public class SomeTests {

    [Test]
    public void FirstFail() {
        Assert.Fail("That is why you fail");
    }

    [Test]
    public void SecondsFail() {
        Assert.Fail("This is also why you fail");
    }

    [Test]
    public void FirstPass() {
        Assert.True(true);
    }

    [Test]
    public void SecondPass() {
        Assert.True(true);
    }

    [Test][Ignore("Not ready ...")]
    public void ShouldEventuallyTestStuff() {
        Assert.Fail();
    }

    [Test][Ignore]
    public void ShouldAlsoEventuallyTestStuff() {
        Assert.Fail();
    }
}
