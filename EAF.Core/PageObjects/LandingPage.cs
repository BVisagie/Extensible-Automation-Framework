using CodenameGenerator;
using EAF.Core.Base;
using OpenQA.Selenium;

namespace EAF.Core.PageObjects
{
    public class LandingPage
    {
        private readonly TestSession sessionVariables;

        public LandingPage(TestSession sessionVariables)
        {
            this.sessionVariables = sessionVariables;
        }

        private readonly By sectionMathematics = By.XPath("//h2/a/span/span[text()='Mathematics']");
        private readonly By sectionScienceTechnology = By.XPath("//h2/a/span/span[text()='Science & Technology']");
        private readonly By sectionSocietyCulture = By.XPath("//h2/a/span/span[text()='Society & Culture']");
        private readonly By sectionEverydayLife = By.XPath("//h2/a/span/span[text()='Everyday Life']");
        private readonly By searchInputBox = By.XPath("//input[@placeholder='Enter what you want to calculate or know about']");

        public bool MainCategoriesLoaded()
        {
            return sessionVariables.ElementDisplayed(sectionMathematics)
                && sessionVariables.ElementDisplayed(sectionScienceTechnology)
                && sessionVariables.ElementDisplayed(sectionSocietyCulture)
                && sessionVariables.ElementDisplayed(sectionEverydayLife);
        }

        public string InputRandomSearchParamater()
        {
            string randomSearchValue = new Generator().Generate();
            sessionVariables.ElementSendKeys(searchInputBox, randomSearchValue);
            return randomSearchValue;
        }

        public LandingPage StartSearchUsingKeyboard()
        {
            sessionVariables.ElementSendKeys(searchInputBox, Keys.Enter);
            return this;
        }
    }
}