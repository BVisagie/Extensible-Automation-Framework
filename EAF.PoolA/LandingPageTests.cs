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
        [Category("Smoke Test Pack")]
        public void TestCase_1()
        {
            sessionVariables = new Session().SetupSession(uiTestCase: true, navigateToUrlUnderTest: true);

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