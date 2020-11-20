using OpenQA.Selenium;
using Serilog.Core;

namespace EAF.Core.Base
{
    internal class SessionProperties
    {
        public IWebDriver Driver { get; set; }
        public Logger Logger { get; set; }
    }
}