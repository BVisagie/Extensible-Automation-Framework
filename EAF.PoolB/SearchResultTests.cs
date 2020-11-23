using EAF.Core.Base;
using EAF.Core.PageObjects;
using NUnit.Framework;

namespace EAF.PoolB
{
    [TestFixture]
    internal class SearchResultTests
    {
        private Session sessionVariables;

        [Test]
        [TestCase(TestName = "Wolframalpha Search Test A")]
        [Description("This test will verify that a search may be completed from the landing page, and that meaningful results are returned.")]
        [Category("Smoke Test Pack")]
        public void TestCase_2()
        {
            sessionVariables = new Session().SetupSession(uiTestCase: true, runHeadless: false);

            sessionVariables.NavigateToUrlUnderTest();

            new LandingPage(sessionVariables)
                .InputRandomSearchParamater()
                .StartSearchUsingKeyboard();
        }

        [TearDown]
        public void Cleanup()
        {
            sessionVariables.TeardownLogic(sessionVariables);
        }
    }
}