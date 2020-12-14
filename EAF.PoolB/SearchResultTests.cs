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
            sessionVariables = new Session().SetupSession(uiTestCase: true, navigateToUrlUnderTest: true);

            var landingPage = new LandingPage(sessionVariables);

            var randomSearchValue = landingPage.InputRandomSearchParamater();
            landingPage.StartSearchUsingKeyboard();

            var searchResultsPage = new SearchResultPage(sessionVariables);
            bool prePopulatedSearchInputCompare = searchResultsPage.VerifyPreviousSearchInput(randomSearchValue);
            Assert.That(prePopulatedSearchInputCompare, Is.True, "The random value searched for on the landing page does not seem to be equal to the pre-populated value on the results page.");
        }

        [TearDown]
        public void Cleanup()
        {
            sessionVariables.TeardownLogic(sessionVariables);
        }
    }
}