using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace Replay.FrontendTests
{
    /// <summary>
    /// Methods to navigate the creation of a <see cref="User"/>
    /// on the <see cref="Views.Account.Register"/>
    /// page
    /// </summary>
    /// <author>Thomas Dworschak, Felix Nebel</author>
    public class CreateUserPage
    {
        private readonly IWebDriver _driver;
        private const string URI = "https://localhost:7102/Account/Register";

        private IWebElement FullName => _driver.FindElement(By.Id("FullName"));
        private IWebElement Email => _driver.FindElement(By.Id("Email"));
        private IWebElement Password => _driver.FindElement(By.Id("Password"));
        private IWebElement CreateElement => _driver.FindElement(By.Id("register"));


        private IWebElement LoginEmail => _driver.FindElement(By.Id("Input_Email"));
        private IWebElement LoginPassword => _driver.FindElement(By.Id("Input_Password"));
        private IWebElement LoginButton => _driver.FindElement(By.Id("login"));

        public string Title => _driver.Title;
        public string Source => _driver.PageSource;
        public string FullNameErrorMessage => _driver.FindElement(By.CssSelector("[data-valmsg-for=\"FullName\"]")).Text;
        public string EmailErrorMessage => _driver.FindElement(By.CssSelector("[data-valmsg-for=\"Email\"]")).Text;
        public string PasswordErrorMessage => _driver.FindElement(By.CssSelector("[data-valmsg-for=\"Password\"]")).Text;


        public CreateUserPage(IWebDriver driver) => _driver = driver;
        public void Navigate() => _driver.Navigate()
            .GoToUrl(URI);

        public void PopulateLoginEmail(string email) => LoginEmail.SendKeys(email);
        public void PopulateLoginPassword(string password) => LoginPassword.SendKeys(password);
        public void ClickLogin() => LoginButton.Click();

        public void PopulateFullName(string name) => FullName.SendKeys(name);
        public void PopulateEmail(string email) => Email.SendKeys(email);
        public void PopulatePassword(string password) => Password.SendKeys(password);
        public void ClickCreate() => CreateElement.Click();
    }
}