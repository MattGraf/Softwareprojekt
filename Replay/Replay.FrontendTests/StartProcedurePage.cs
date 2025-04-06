using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using Xunit;

namespace Replay.FrontendTests
{
    /// <summary>
    /// Navigates through the pages required for testing the
    /// start of a <see cref="Procedure"/> based on a <see cref="Process"/>
    /// It contains the initial URI and the check,
    /// if the proedure view contains the initiated process
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class StartProcedurePage
    {
        private readonly IWebDriver _driver;
        private const string ProcessIndex = "https://localhost:7102/MakandraProcess/Index";

        List<string> procedureRowContent = new List<string>
        {
            "Onboarding", "01.08.2024"
        };

        private IWebElement LoginEmail => _driver.FindElement(By.Id("Input_Email"));
        private IWebElement LoginPassword => _driver.FindElement(By.Id("Input_Password"));
        private IWebElement LoginButton => _driver.FindElement(By.Id("login"));

        public StartProcedurePage(IWebDriver driver) => _driver = driver;

        public void InitialNavigate() => _driver.Navigate().GoToUrl(ProcessIndex);

        public void PopulateLoginEmail(string email) => LoginEmail.SendKeys(email);
        public void PopulateLoginPassword(string password) => LoginPassword.SendKeys(password);
        public void ClickLogin() => LoginButton.Click();

        public void CheckInitiatedProcess()
        {
            var initiatedProcessRow = _driver.FindElement(By.XPath("//table/tbody/tr[td[1][contains(text(), 'Onboarding')]]"));
            var dateCell = initiatedProcessRow.FindElement(By.XPath("td[2]"));
            var dateText = dateCell.Text;

            Assert.Equal(procedureRowContent[0], initiatedProcessRow.FindElement(By.XPath("td[1]")).Text);
            Assert.Equal(procedureRowContent[1], dateText);
        }
    }
}
