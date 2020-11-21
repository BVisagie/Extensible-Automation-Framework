using EAF.Core.Base;
using EAF.Core.PageObjects;
using NUnit.Framework;

namespace EAF.PoolA
{
    [TestFixture]
    internal class LandingPageTests
    {
        private Session sessionVariables;

        [Test]
        [TestCase(TestName = "Wolframalpha Landing Page Test A")]
        [Description("This test will verify that the four main categories on the Wolframalpha landing page is loaded.")]
        [Category("[CLM] Regression Test Pack")]
        public void Test_Case_43717()
        {
            sessionVariables = new Session().SetupSession(uiTestCase: true, runHeadless: false);

            sessionVariables.NavigateToUrlUnderTest();

            var pageCategoriesLoaded = new LandingPage(sessionVariables).MainCategoriesLoaded();
            Assert.That(pageCategoriesLoaded, Is.True, "A problem has been encountered with the verification of the four main categories on the Wolframalpha landing page.");
        }

        [TearDown]
        public void Cleanup()
        {
            sessionVariables.TeardownLogic(sessionVariables);
        }
    }
}