using EAF.Core.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;

namespace EAF.Core.Base
{
    public class SessionProperties
    {
        public IWebDriver Driver { get; set; }
        public WebDriverWait DriverWait { get; set; }
        public string UrlUnderTest { get; set; }
        public string TargetBrowser { get; set; }
        public bool UiTestCase { get; set; }
        public bool EnableIncognito { get; set; }
        public ILogger Logger { get; set; }
        public string LoggerUid { get; set; }
        public bool PipelineRun { get; set; }
        public SharedMethods SharedMethods { get; set; }
        public string TestName { get; set; }
        public bool RunHeadless { get; set; }
    }
}