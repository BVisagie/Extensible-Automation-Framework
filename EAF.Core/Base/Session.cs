using EAF.Core.Utilities;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;

namespace EAF.Core.Base
{
    public class Session : SessionProperties
    {
        /// <summary>
        /// This method will setup the base variables for both UI and non-UI test cases.
        /// </summary>
        public Session SetupSession(bool uiTestCase, bool runHeadless)
        {
            Driver = null;

            UrlUnderTest = TestContext.Parameters["ApplicationURL"];

            TestName = TestContext.CurrentContext.Test.Name;

            RunHeadless = runHeadless;

            SharedMethods = new SharedMethods();

            Logger = SetupLogger();
            Logger.Debug("Logger setup complete.");

            UiTestCase = uiTestCase;
            Logger.Debug($"UI Test Case: {UiTestCase}");

            PipelineRun = string.Equals(TestContext.Parameters["PipelineRun"], "true", StringComparison.OrdinalIgnoreCase);
            Logger.Debug($"Pipe line run: {PipelineRun}");

            EnableIncognito = string.Equals(TestContext.Parameters["EnableIncognito"], "true", StringComparison.OrdinalIgnoreCase);
            Logger.Debug($"Enable incognito: {EnableIncognito}");

            if (UiTestCase)
            {
                TargetBrowser = TestContext.Parameters["TargetBrowser"];
                Logger.Debug($"Target browser: {TargetBrowser}");

                if (TargetBrowser == "Chrome")
                {
                    Logger.Debug("Starting setup of Chrome driver.");
                    Driver = SetupChromeWebDriver();
                }
                else if (TargetBrowser == "Edge")
                {
                    Logger.Debug("Starting setup of Edge driver.");
                    Driver = SetupEdgeWebDriver();
                }
            }

            DriverWait = SetupDriverWait();

            //return the instance of the session class
            return this;
        }

        /// <summary>
        /// This method will setup and return a configured ILogger logging instance.
        /// It will also create the log file, using the naming convention: testcasename-uniqueid
        /// </summary>
        /// <param name="testCaseName"></param>
        /// <returns>ILogger</returns>
        private ILogger SetupLogger()
        {
            var a = SharedMethods.ShortUid();

            LoggerUid = $"{TestName}-{a}";
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($"{LoggerUid}.txt")
                .CreateLogger();
        }

        private IWebDriver SetupChromeWebDriver()
        {
            ChromeOptions chromeOptions = new ChromeOptions();

            if (EnableIncognito)
            {
                chromeOptions.AddArgument("--incognito");
            }

            if (RunHeadless)
            {
                chromeOptions.AddArgument("--headless");
            }

            if (PipelineRun)
            {
                chromeOptions.AddArgument("--disable-gpu");
                chromeOptions.AddArgument("--window-size=1920,960");
            }
            else
            {
                chromeOptions.AddArgument("--start-maximized");
            }

            chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.Debug);
            chromeOptions.SetLoggingPreference(LogType.Driver, LogLevel.Debug);

            Logger.Debug("Now loading Chrome options:");
            Logger.Debug("{@ChromeOptions}", chromeOptions);
            Driver = new ChromeDriver(chromeOptions);

            return Driver;
        }

        private IWebDriver SetupEdgeWebDriver()
        {
            EdgeOptions edgeOptions = new EdgeOptions
            {
                UseChromium = true
            };

            edgeOptions.BinaryLocation = TestContext.Parameters["EdgeBrowserBinaryLocation"];
            Logger.Debug($"Attempting to find Edge .exe here: {TestContext.Parameters["EdgeBrowserBinaryLocation"]}");

            if (RunHeadless)
            {
                edgeOptions.AddArgument("headless");
            }

            if (PipelineRun)
            {
                edgeOptions.AddArgument("disable-gpu");
                edgeOptions.AddArgument("window-size=1920,960");
            }
            else
            {
                edgeOptions.AddArgument("start-maximized");
            }

            edgeOptions.SetLoggingPreference(LogType.Browser, LogLevel.Debug);
            edgeOptions.SetLoggingPreference(LogType.Driver, LogLevel.Debug);

            Logger.Debug("Now loading Edge options:");
            Logger.Debug("{@EdgeOptions}", edgeOptions);
            Driver = new EdgeDriver(edgeOptions);

            return Driver;
        }

        public WebDriverWait SetupDriverWait(int webDriverWait = 5000)
        {
            return new WebDriverWait(Driver, TimeSpan.FromMilliseconds(webDriverWait));
        }

        /// <summary>
        /// This method will wait for a drop down to become visible then returns a usable SelectElement data type.
        /// </summary>
        /// <param name="theElementBy"></param>
        public SelectElement SelectElement(By theElementBy)
        {
            try
            {
                return new SelectElement(DriverWait.Until(e => e.FindElement(theElementBy)));
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will wait for a drop down to become visible then selects the given elment by full or partial string.
        /// </summary>
        /// <param name="theElementBy">The element to make the selection on.</param>
        /// <param name="elementText">The element full or partial text to use to make the selection.</param>
        /// <param name="partialMatch">Determines if a full or partial text match should be used when making the selection.
        /// The default will be set to false and will result in a full text match unless set to true.
        /// </param>
        public void SelectElementByText(By theElementBy, string elementText, bool partialMatch = false)
        {
            if (partialMatch)
            {
                Logger.Debug($"Attempting to select a drop down by element partial text, element: {theElementBy}, element partial text option must contain: {elementText}");
            }
            else
            {
                Logger.Debug($"Attempting to select a drop down by element text, element: {theElementBy}, element text option: {elementText}");
            }

            try
            {
                new SelectElement(DriverWait.Until(e => e.FindElement(theElementBy))).SelectByText(elementText, partialMatch: partialMatch);
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will wait for a drop down to become visible then selects the given elment by index.
        /// </summary>
        /// <param name="elementIndex"></param>
        public void SelectElementByIndex(By theElementBy, int elementIndex)
        {
            Logger.Debug($"Attempting to select a drop down by it's index position, element: {theElementBy}, index option: {elementIndex}");

            try
            {
                new SelectElement(DriverWait.Until(e => e.FindElement(theElementBy))).SelectByIndex(elementIndex);
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will return the selected drop down option as a string
        /// </summary>
        /// <param name="theElementBy"></param>
        public string SelectElementText(By theElementBy)
        {
            Logger.Debug($"Attempting to return the selected drop down option as a string: {theElementBy}");

            try
            {
                return new SelectElement(DriverWait.Until(e => e.FindElement(theElementBy))).SelectedOption.Text;
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will wait for the given element to become clickable then it will click the element.
        /// </summary>
        /// <param name="theElementBy"></param>
        public void ElementClick(By theElementBy, int numberOfClicks = 1)
        {
            Logger.Debug($"Attempting to click element: {theElementBy}");

            try
            {
                if (numberOfClicks > 1)
                {
                    for (int i = 0; i < numberOfClicks; i++)
                    {
                        DriverWait.Until(e => e.FindElement(theElementBy)).Click();
                    }
                }
                else
                {
                    DriverWait.Until(e => e.FindElement(theElementBy)).Click();
                }
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// This method  will wait for the given element to become visible then it will send the given keys.
        /// </summary>
        /// <param name="theElementBy"></param>
        public void ElementSendKeys(By theElementBy, string theKey)
        {
            Logger.Debug($"Attempting to send keys to an element, element: {theElementBy}, keys: {theKey}");

            try
            {
                DriverWait.Until(e => e.FindElement(theElementBy)).SendKeys(theKey);
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will wait for the given element to become visible then it will clear the element.
        /// </summary>
        /// <param name="theElementBy"></param>
        public void ElementClearInput(By theElementBy)
        {
            Logger.Debug($"Attempting to clear an input element, element: {theElementBy}.");

            try
            {
                DriverWait.Until(e => e.FindElement(theElementBy)).Clear();
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will return the text for a given element
        /// </summary>
        /// <param name="theElementBy"></param>
        public string ElementText(By theElementBy)
        {
            Logger.Debug($"Attempting to get text from en element, target element: {theElementBy}");

            try
            {
                return DriverWait.Until(e => e.FindElement(theElementBy)).Text;
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will return the Attribute value for a given element
        /// </summary>
        /// <param name="theElementBy"></param>
        public string ElementAttributeValue(By theElementBy, string htmlAttribute = "value")
        {
            Logger.Debug($"Attempting to get element attribute value, target element: {theElementBy}, attribute: {htmlAttribute}");

            try
            {
                return DriverWait.Until(e => e.FindElement(theElementBy)).GetAttribute(htmlAttribute);
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will return the Attribute value for a given element
        /// </summary>
        /// <param name="theElementBy"></param>
        public string ElementCssValue(By theElementBy, string cssAttribute)
        {
            Logger.Debug($"Attempting to get element css value, target element: {theElementBy}, css attribute: {cssAttribute}");

            try
            {
                return DriverWait.Until(e => e.FindElement(theElementBy)).GetCssValue(cssAttribute);
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will check if a given element is displayed, returning a bool
        /// </summary>
        /// <param name="theElementBy"></param>
        public bool ElementDisplayed(By theElementBy)
        {
            Logger.Debug($"Attempting to check if element is displayed, target element: {theElementBy}");

            try
            {
                return DriverWait.Until(e => e.FindElement(theElementBy)).Displayed;
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is NoSuchElementException
            )
            {
                return false;
            }
        }

        /// <summary>
        /// This method will check if a given element is selected, returning a bool
        /// </summary>
        /// <param name="theElementBy"></param>
        public bool ElementSelected(By theElementBy)
        {
            Logger.Debug($"Attempting to check if element is selected, target element: {theElementBy}");

            try
            {
                return DriverWait.Until(e => e.FindElement(theElementBy)).Selected;
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is NoSuchElementException
            )
            {
                return false;
            }
        }

        /// <summary>
        /// This method will check if a given element is enabled, returning a bool
        /// </summary>
        /// <param name="theElementBy"></param>
        public bool ElementEnabledCheck(By theElementBy)
        {
            Logger.Debug($"Attempting to check if element is enabled, target element: {theElementBy}");

            try
            {
                return DriverWait.Until(e => e.FindElement(theElementBy)).Enabled;
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is NoSuchElementException
            )
            {
                return false;
            }
        }

        /// <summary>
        /// This method will return a count of elements after waiting for visibility of object
        /// </summary>
        /// <param name="theElementBy"></param>
        public int ElementCount(By theElementBy)
        {
            Logger.Debug($"Attempting to check element count, target element: {theElementBy}");

            try
            {
                return DriverWait.Until(e => e.FindElements(theElementBy)).Count;
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will return a count of elements
        /// </summary>
        /// <param name="theElementBy"></param>
        public int ElementCountNotWaitingForVisibility(By theElementBy)
        {
            try
            {
                return Driver.FindElements(theElementBy).Count;
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will wait for an element to become visible
        /// </summary>
        /// <param name="theElementBy"></param>
        public IWebElement WaitForElementToBeVisible(By theElementBy)
        {
            try
            {
                return DriverWait.Until(e => e.FindElement(theElementBy));
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will wait for a drop down to become visible
        /// Then count the total number of options within the drop down
        /// It will then select a random drop down item by index.
        /// </summary>
        /// <param name="theElementBy"></param>
        public void RandomDropDownSelection(By theElementBy)
        {
            try
            {
                var optionsListCount = new SelectElement(DriverWait.Until(e => e.FindElement(theElementBy))).Options.Count;
                new SelectElement(DriverWait.Until(e => e.FindElement(theElementBy))).SelectByIndex(SharedMethods.GetRandomNumber(min: 1, max: optionsListCount));
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        /// <summary>
        /// Selects an element from a drop down box other than what is specified.
        /// Example if we have a list of animals {Dog, Wolf, Bear, Fox} and Bear is
        /// specified in the 'optionNotToSelect' param than anything other than Bear will be selected.
        /// </summary>
        /// <param name="theElementBy">The drop down element to make selection.</param>
        /// <param name="optionNotToSelect">The option contained within drop down not to select.</param>
        public void SelectDropdownOptionOtherThanSpecified(By theElementBy, string optionNotToSelect)
        {
            try
            {
                var dropDownElement = new SelectElement(DriverWait.Until(e => e.FindElement(theElementBy)));
                int dropDownOptionsListCount = new SelectElement(DriverWait.Until(e => e.FindElement(theElementBy))).Options.Count;
                var dropdownOptionsList = dropDownElement.Options;

                for (int i = 1; i < dropDownOptionsListCount; i++)
                {
                    if (dropdownOptionsList[i].Text != optionNotToSelect)
                    {
                        dropDownElement.SelectByText(dropdownOptionsList[i].Text);
                        break;
                    }
                }
            }
            catch (Exception exception) when (exception is TimeoutException
            || exception is ElementNotInteractableException
            || exception is ElementNotVisibleException
            || exception is ElementNotSelectableException
            || exception is StaleElementReferenceException
            || exception is NoSuchElementException)
            {
                throw;
            }
        }

        public void TeardownLogic(Session sessionVariable)
        {
            if (TestContext.CurrentContext.Result.Outcome != ResultState.Success && TestContext.CurrentContext.Result.Outcome != ResultState.Inconclusive)
            {
                //Log the exception to disk and if it's ui test case we should take a screenshot
                sessionVariable.LogTestCaseException();
            }
            else
            {
                //If no exception occured we can still attach the running log file
                //this should assist in tracking test data
                //you cannot log an exception if the session variable was not created
                sessionVariable?.AttachCurrentTestLogFile();
            }

            if (sessionVariable?.UiTestCase == true)
            {
                sessionVariable.Driver?.Quit();
            }
        }

        public void LogTestCaseException()
        {
            foreach (var logItem in TestContext.CurrentContext.Result.Assertions)
            {
                Logger.Error($"Assertions.Status: {logItem.Status}");
                Logger.Error($"Assertions.Message: {logItem.Message}");
                Logger.Error($"Assertions.StackTrace: {logItem.StackTrace}");
            }

            TestContext.AddTestAttachment($"{LoggerUid}.txt");

            if (UiTestCase)
            {
                ITakesScreenshot ts = Driver as ITakesScreenshot;
                Screenshot screenshot = ts.GetScreenshot();
                string screenshotFilePathAndName = $"{LoggerUid}.png";
                screenshot.SaveAsFile(screenshotFilePathAndName, ScreenshotImageFormat.Png);
                TestContext.AddTestAttachment(screenshotFilePathAndName);
            }
        }

        /// <summary>
        /// This method attaches the current log file to a passing test case.
        /// The intention of this is to allow a test engineer to be able to source a passing test cases test data easily on Azure DevOps
        /// </summary>
        public void AttachCurrentTestLogFile()
        {
            TestContext.AddTestAttachment($"{LoggerUid}.txt");
        }

        public void NavigateToUrlUnderTest()
        {
            Driver.Navigate().GoToUrl(UrlUnderTest);
        }
    }
}