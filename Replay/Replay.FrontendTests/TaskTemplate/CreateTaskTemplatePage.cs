using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V125.FedCm;
using OpenQA.Selenium.Support.UI;

namespace Replay.FrontendTests.TaskTemplate
{
    /// <summary>
    /// Initlialize TaskTemplatePage for the TaskTemplate-FrontendTests
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class CreateTaskTemplatePage
    {
        private readonly IWebDriver _driver;
        private const string URI = "https://localhost:7102/TaskTemplate/Create";

        private IWebElement LoginEmail => _driver.FindElement(By.Id("Input_Email"));
        private IWebElement LoginPassword => _driver.FindElement(By.Id("Input_Password"));
        private IWebElement LoginButton => _driver.FindElement(By.Id("login"));
        private IWebElement CreateElement => _driver.FindElement(By.Id("Create"));

        private IWebElement Name => _driver.FindElement(By.Id("Name"));
        private IWebElement Instruction => _driver.FindElement(By.Id("Instruction"));
        public IWebElement MakandraProcessId => _driver.FindElement(By.Id("MakandraProcessId"));

        
        public string NameErrorMessage => _driver.FindElement(By.CssSelector("[data-valmsg-for=\"Name\"]")).Text;
        public string ProcessErrorMessage => _driver.FindElement(By.CssSelector("[data-valmsg-for=\"MakandraProcessId\"]")).Text;

        public CreateTaskTemplatePage(IWebDriver driver) => _driver = driver;
        public void Navigate() => _driver.Navigate()
            .GoToUrl(URI);
        

        public void PopulateLoginEmail(string email) => LoginEmail.SendKeys(email);
        public void PopulateLoginPassword(string password) => LoginPassword.SendKeys(password);
        public void ClickLogin() => LoginButton.Click();

        public void PopulateName(string name) => Name.SendKeys(name);
        public void PopulateInstruction(string instruction) => Instruction.SendKeys(instruction);
        public void ClickCreate() => CreateElement.Click();
    }
}