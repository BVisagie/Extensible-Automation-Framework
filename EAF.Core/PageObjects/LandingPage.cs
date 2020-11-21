using EAF.Core.Base;
using OpenQA.Selenium;

namespace EAF.Core.PageObjects
{
    public class LandingPage
    {
        private readonly Session sessionVariables;

        public LandingPage(Session sessionVariables)
        {
            this.sessionVariables = sessionVariables;
        }

        private readonly By sectionMathematics = By.XPath("//h2/a/span/span[text()='Mathematics']");
        private readonly By sectionScienceTechnology = By.XPath("//h2/a/span/span[text()='Science & Technology']");
        private readonly By sectionSocietyCulture = By.XPath("//h2/a/span/span[text()='Society & Culture']");
        private readonly By sectionEverydayLife = By.XPath("//h2/a/span/span[text()='Everyday Life']");

        public bool MainCategoriesLoaded()
        {
            return sessionVariables.ElementDisplayed(sectionMathematics) && sessionVariables.ElementDisplayed(sectionScienceTechnology) && sessionVariables.ElementDisplayed(sectionSocietyCulture) && sessionVariables.ElementDisplayed(sectionEverydayLife);
        }
    }
}