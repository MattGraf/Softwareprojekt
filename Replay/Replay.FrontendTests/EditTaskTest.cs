using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace Replay.FrontendTests 
{
    /// <author>Florian Fendt</author>
    public class EditTaskTest: IDisposable
    {
        private readonly IWebDriver driver;
        public EditTaskTest()
        {
            ChromeOptions options = new ChromeOptions();
            options.AcceptInsecureCertificates = true;
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--window-size=1920x1080");
            options.AddArgument("--disable-gpu");
            driver = new ChromeDriver(options);
        }
        public void Dispose()
        {
            driver.Quit();
            driver.Dispose();
        }
        /// <summary>
        /// Checks if the Changes are done correctly when task is edited
        /// </summary>
        [Fact]
        public void Edit_Task()
        {
            driver.Navigate().GoToUrl("https://localhost:7102/MakandraTask/Index");
            IWebElement emailField = driver.FindElement(By.Id("Input_Email"));
            IWebElement passwordField = driver.FindElement(By.Id("Input_Password"));
            IWebElement loginButton = driver.FindElement(By.Id("login"));
            emailField.SendKeys("bill@replay.de");
            passwordField.SendKeys("B1llard!");
            loginButton.Click();
            IWebElement editButton = driver.FindElement(By.CssSelector("[href=\"/MakandraTask/Edit/123\"]"));
            editButton.Click();
            Assert.Equal("https://localhost:7102/MakandraTask/Edit/123", driver.Url);
            IWebElement nameField = driver.FindElement(By.Id("Name"));
            nameField.Clear();
            nameField.SendKeys("Testname");
            IWebElement notesField = driver.FindElement(By.Id("Notes"));
            notesField.Clear();
            notesField.SendKeys("Das ist ein Test");
            IWebElement instructionField = driver.FindElement(By.Id("Instruction"));
            instructionField.Clear();
            instructionField.SendKeys("Neue Anleitungen für die Aufgabe");
            IWebElement stateDropdown = driver.FindElement(By.Name("SelectedStateId"));
            Actions actions = new Actions(driver);
            actions.MoveToElement(stateDropdown).Click().Perform();
            IWebElement stateOption = stateDropdown.FindElement(By.XPath($"//option[@value='{"2"}']"));
            stateOption.Click();
            actions.MoveToElement(driver.FindElement(By.Id("SelectedRoleId")));
            var selectElement = driver.FindElement(By.Id("SelectedRoleId"));
            var select = new SelectElement(selectElement);
            driver.FindElement(By.CssSelector("option[value='5']"));
            select.SelectByText("Backoffice");
            IWebElement selectedOption = select.SelectedOption;
            IWebElement editorCheckbox = driver.FindElement(By.XPath("//input[@name='PossibleEditors[0].IsSelected']"));
            actions.MoveToElement(editorCheckbox).Perform();
            if (!editorCheckbox.Selected)
            {
                actions.Click(editorCheckbox).Perform();
            }
            IWebElement submitButton = driver.FindElement(By.Id("edit"));
            actions.MoveToElement(submitButton).Perform();
            submitButton.Click();
            IWebElement detailsButton = driver.FindElement(By.CssSelector("[href=\"/MakandraTask/Detail/123\"]"));                   
            detailsButton.Click();
            IWebElement nameElement = driver.FindElement(By.CssSelector("h2"));
            IWebElement stateElement = driver.FindElement(By.Id("state"));
            IWebElement roleElement = driver.FindElement(By.Id("role"));
            IWebElement notesElement = driver.FindElement(By.Id("notes"));
            IWebElement instructionElement = driver.FindElement(By.Id("instruction"));
            IWebElement editAccessElement = driver.FindElement(By.Id("editaccess"));
            string expectedName = "Testname";
            string expectedState = "In Bearbeitung";
            string expectedRole = "Backoffice";
            string expectedNotes = "Das ist ein Test";
            string expectedInstruction = "Neue Anleitungen für die Aufgabe";
            string expectedEditAccess = "Administrator";
            Assert.Equal(expectedName, nameElement.Text);
            Assert.Equal(expectedState, stateElement.Text);
            Assert.Equal(expectedRole, roleElement.Text);
            Assert.Equal(expectedNotes, notesElement.Text);
            Assert.Equal(expectedInstruction, instructionElement.Text);
            Assert.Equal(expectedEditAccess, editAccessElement.Text);
        }
        /// <summary>
        /// Checks if ErrorMessages are displayed if nothing is provided
        /// </summary>
        [Fact]
        public void Edit_Task2()
        {
            driver.Navigate().GoToUrl("https://localhost:7102/MakandraTask/Index");
            IWebElement emailField = driver.FindElement(By.Id("Input_Email"));
            IWebElement passwordField = driver.FindElement(By.Id("Input_Password"));
            IWebElement loginButton = driver.FindElement(By.Id("login"));
            emailField.SendKeys("bill@replay.de");
            passwordField.SendKeys("B1llard!");
            loginButton.Click();
            IWebElement editButton = driver.FindElement(By.CssSelector("[href=\"/MakandraTask/Edit/123\"]"));
            editButton.Click();
            IWebElement nameField = driver.FindElement(By.Id("Name"));
            nameField.Clear();
            IWebElement targetDateField = driver.FindElement(By.Id("TargetDate"));
            targetDateField.Clear();
            IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            Actions actions = new Actions(driver);
            actions.MoveToElement(submitButton).Perform();
            submitButton.Click();
            string DescriptorErrorMessage = driver.FindElement(By.CssSelector("[data-valmsg-for=\"Name\"]")).Text;
            Assert.Equal("Ein Name muss angegeben werden", DescriptorErrorMessage);
        }
    }
}