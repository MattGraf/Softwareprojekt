using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Replay.FrontendTests
{
    public class AdminProcedureOverviewPage
    {
        private readonly IWebDriver _driver;
        private const string URI = "https://localhost:7102/Procedure/AdminIndex";

        private IWebElement Descriptor => _driver.FindElement(By.Id("Name"));
        private IWebElement CreateElement => _driver.FindElement(By.Id("create"));

        private IWebElement LoginEmail => _driver.FindElement(By.Id("Input_Email"));
        private IWebElement LoginPassword => _driver.FindElement(By.Id("Input_Password"));
        private IWebElement LoginButton => _driver.FindElement(By.Id("login"));

        public string Title => _driver.Title;
        public string Source => _driver.PageSource;
        public string DescriptorErrorMessage => _driver.FindElement(By.CssSelector("[data-valmsg-for=\"Name\"]")).Text;


        public AdminProcedureOverviewPage(IWebDriver driver) => _driver = driver;
        public void Navigate() => _driver.Navigate()
            .GoToUrl(URI);


        public void PopulateLoginEmail(string email) => LoginEmail.SendKeys(email);
        public void PopulateLoginPassword(string password) => LoginPassword.SendKeys(password);
        public void ClickLogin() => LoginButton.Click();
    }
}