using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using OpenQA.Selenium;

namespace Replay.FrontendTests
{
    public class ViewTasksPage
    {
        private readonly IWebDriver _driver;
        private const string URI = "https://localhost:7102/Account/Login?ReturnUrl=%2FMakandraTask%2FIndex";

        public List<List<String>> processResponsibleRowContent = new List<List<String>>()
        {
            new List<String>()
            {
                "Information zu Rahmenbedingungen des Vertrags einholen", "Neuzugang HR", "26.07.2024 (Überfällig)", "Offen", "Details"
            },
            new List<String>()
            {
                "Vertrag erstellen", "Neuzugang HR", "26.07.2024 (Überfällig)", "Offen", "Details"
            },
            new List<String>()
            {
                "Vertrag vor ab neuen Mitarbeitenden per Mail senden", "Neuzugang HR", "26.07.2024 (Überfällig)", "Offen", "Details"
            }
        };



        private IWebElement LoginEmail => _driver.FindElement(By.Id("Input_Email"));
        private IWebElement LoginPassword => _driver.FindElement(By.Id("Input_Password"));
        private IWebElement LoginButton => _driver.FindElement(By.Id("login"));

        private IWebElement ProcessResposibleTable => _driver.FindElement(By.Id("user"));

        public string Title => _driver.Title;
        public string Source => _driver.PageSource;
        public string DescriptorErrorMessage => _driver.FindElement(By.CssSelector("[data-valmsg-for=\"Name\"]")).Text;


        public ViewTasksPage(IWebDriver driver) => _driver = driver;
        public void Navigate() => _driver.Navigate()
            .GoToUrl(URI);

        public void PopulateLoginEmail(string email) => LoginEmail.SendKeys(email);
        public void PopulateLoginPassword(string password) => LoginPassword.SendKeys(password);
        public void ClickLogin() => LoginButton.Click();

        /*public void CheckUserSpecificTable()
        {
            int i = 0, j = 0;
            List<IWebElement> userspecificrows = UserSpecificTable.FindElements(By.TagName("tr")).ToList();
            foreach(IWebElement row in userspecificrows)
            {
                var columnsList = row.FindElements(By.TagName("td"));
                foreach(IWebElement column in columnsList)
                {
                    Assert.Contains(userRowContent[i][j], column.Text);
                    j++;
                }
                if (j > 0)
                    i++;
                j = 0;
            }
        }*/

        public void CheckProcessResponsibleTable()
        {
            int i = 0, j = 0;
            List<IWebElement> userspecificrows = ProcessResposibleTable.FindElements(By.TagName("tr")).ToList();
            foreach (IWebElement row in userspecificrows)
            {
                var columnsList = row.FindElements(By.TagName("td"));
                foreach (IWebElement column in columnsList)
                {
                    Assert.Contains(processResponsibleRowContent[i][j], column.Text);
                    j++;
                }
                if (j > 0)
                    i++;
                j = 0;
                if (i >= processResponsibleRowContent.Count)
                    break;
            }
        }

        /*public void CheckRoleSpecificTable()
        {
            int i = 0, j = 0;
            List<IWebElement> userspecificrows = RoleSpecificTable.FindElements(By.TagName("tr")).ToList();
            foreach (IWebElement row in userspecificrows)
            {
                var columnsList = row.FindElements(By.TagName("td"));
                foreach (IWebElement column in columnsList)
                {
                    Assert.Contains(roleSpecificRowContent[i][j], column.Text);
                    j++;
                }
                if (j > 0)
                    i++;
                j = 0;
            }
        }*/


    }
}
