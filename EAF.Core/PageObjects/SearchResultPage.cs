using EAF.Core.Base;
using OpenQA.Selenium;

namespace EAF.Core.PageObjects
{
    public class SearchResultPage
    {
        private readonly Session sessionVariables;

        public SearchResultPage(Session sessionVariables)
        {
            this.sessionVariables = sessionVariables;
        }

        private readonly By searchInputByPlaceHodler = By.XPath("//input[@placeholder='Enter what you want to calculate or know about']");
        private readonly By previousSearchInput = By.XPath("//input[@placeholder='Enter what you want to calculate or know about']/preceding-sibling::span");

        /// <summary>
        /// This method will return a bool based on whether the input string (from the landing page) is equal to the prepopulated value on the results page.
        /// </summary>
        /// <param name="randomSearchValue"></param>
        /// <returns></returns>
        public bool VerifyPreviousSearchInput(string randomSearchValue)
        {
            return sessionVariables.ElementText(previousSearchInput) == randomSearchValue;
        }
    }
}