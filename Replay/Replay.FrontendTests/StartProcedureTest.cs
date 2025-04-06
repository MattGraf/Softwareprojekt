using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace Replay.FrontendTests
{
    /// <summary>
    /// Checks, if a <see cref="Procedure"/> can be started
    /// based on the existing <see cref="Process"/> <c>Onboarding</c>
    /// with id <c>1</c> by a <see cref="User"/> with approriate rights.
    /// And if a procedure can not be initiated by a user with
    /// inappropriate rights
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class StartProcedureTest : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly StartProcedurePage _page;
        private const string ProcedureIndex = "https://localhost:7102/Procedure/Index";

        public StartProcedureTest()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--window-size=1920x1080");

            _driver = new ChromeDriver(options);
            _page = new StartProcedurePage(_driver);
            _page.InitialNavigate();
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        /// <summary>
        /// Checks, if a <see cref="User"/> with appropriate rights
        /// can start a <see cref="Process"/> with Id <c>1</c>,
        /// and if a corresponding <see cref="Procedure"/> has
        /// been initiated and is visible in the index view
        /// </summary>
        /// <author>Thomas Dworschak</author>
        [Fact]
        public void StartProcedure_Successfully()
        {
            Login_CorrectAccess();

            AssertUrlContains("MakandraProcess/Index");

            IWebElement startButton = _driver.FindElement(By.CssSelector("[href=\"/MakandraProcess/Start/1\"]"));
            startButton.Click();

            AssertUrlContains("MakandraProcess/Start");

            Actions actions = new Actions(_driver);

            actions.MoveToElement(_driver.FindElement(By.Id("StartProcedure"))).Build().Perform();


            _driver.FindElement(By.Id("StartProcedure")).Click();

            AssertUrlContains("Procedure/Index");

            _driver.Navigate().GoToUrl(ProcedureIndex);
            Console.WriteLine($"Navigated to {ProcedureIndex}");

            _page.CheckInitiatedProcess();
        }

        /// <summary>
        /// Checks, if a <see cref="User"/> with inappropriate rights
        /// can not start a <see cref="Process"/> with Id <c>1</c>,
        /// and therefore remains in the index view of <see cref="Process"/>
        /// </summary>
        /// <author>Thomas Dworschak</author>
        [Fact]
        public void StartProcedure_Unsuccessfully()
        {
            Login_IncorrectAccess();

            AssertUrlContains("MakandraProcess/Index");

            IWebElement startButton = _driver.FindElement(By.CssSelector("[href=\"/MakandraProcess/Start/1\"]"));
            startButton.Click();

            AssertUrlContains("MakandraProcess/Index");            
        }

        /// <summary>
        /// Login of a <see cref="User"/> that has the
        /// rights to initiated the <see cref="Process"/>
        /// </summary>
        /// <author>Thomas Dworschak</author>
        private void Login_CorrectAccess()
        {
            var email = "bill@replay.de";
            var password = "B1llard!";

            _page.PopulateLoginEmail(email);
            _page.PopulateLoginPassword(password);
            _page.ClickLogin();

        }

        /// <summary>
        /// Login of a <see cref="User"/> that does
        /// not have the rights to initiate the <see cref="Process"/>
        /// </summary>
        /// <author>Thomas Dworschak</author>
        private void Login_IncorrectAccess()
        {
            var email = "gerhard@replay.de";
            var password = "Gerhard1!";

            _page.PopulateLoginEmail(email);
            _page.PopulateLoginPassword(password);
            _page.ClickLogin();
        }

        /// <summary>
        /// Helper method to check, if navigation through the
        /// pages is correct
        /// </summary>
        /// <param name="expectedUrlFragment">URL that represents to correct navigation</param>
        /// <author>Thomas Dworschak</author>
        private void AssertUrlContains(string expectedUrlFragment)
        {
            var currentUrl = _driver.Url;
            Assert.Contains(expectedUrlFragment, currentUrl);
        }
    }
}
