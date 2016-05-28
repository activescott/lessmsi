using Xunit;

namespace LessMsi.Tests
{
    // We use Test Collections to prevent files get locked by seperate threads: http://xunit.github.io/docs/running-tests-in-parallel.html
    [Collection("NUnit - 2.5.2.9222.msi")]
    public class MiscTestsNunit : TestBase
    {
        [Fact]
        public void NUnit()
        {
            ExtractAndCompareToMaster("NUnit-2.5.2.9222.msi");
        }
    }
}
