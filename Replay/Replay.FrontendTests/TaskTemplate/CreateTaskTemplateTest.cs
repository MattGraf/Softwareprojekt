using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.DevTools.V125.FedCm;

namespace Replay.FrontendTests.TaskTemplate
{
    /// <summary>
    /// Frontend-Tests to create <see cref="TaskTemplate">s
    /// </summary>
    public class CreateTaskTemplateTest : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly CreateTaskTemplatePage _page;

        /// <summary>
        /// Initialize options for the TaskTemplate-FrontendTests
        /// </summary>
        /// <author>Matthias Grafberger</author>
        public CreateTaskTemplateTest()
        {

            ChromeOptions options = new ChromeOptions();
            options.AcceptInsecureCertificates = true;
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--window-size=1920x1080");
            options.AddArgument("--disable-gpu");

            _driver = new ChromeDriver(options);
            _page = new CreateTaskTemplatePage(_driver);
            _page.Navigate();
        }

        /// <summary>
        /// End the Frontend-Tests for <see cref="TaskTemplate"/>
        /// </summary>
        /// <author>Matthias Grafberger</author>
        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        /// <summary>
        /// Tests a correct login
        /// </summary>
        /// <author>Felix Nebel</author>
        [Fact]
        public void Create_Login_Correct()
        {
            Assert.Equal("https://localhost:7102/Account/Login?ReturnUrl=%2FTaskTemplate%2FCreate", _driver.Url);
            _page.PopulateLoginEmail("admin@replay.de");
            _page.PopulateLoginPassword("Admin1!");
            _page.ClickLogin();
            Assert.Equal("https://localhost:7102/TaskTemplate/Create", _driver.Url);
        }

        /// <summary>
        /// Tests if it is needed to be logged in to create a <see cref="TaskTemplate"/>
        /// </summary>
        /// <author>Matthias Grafberger</author>
        [Fact]
        public void Create_ReturnCreateView()
        {
            Assert.Equal("https://localhost:7102/Account/Login?ReturnUrl=%2FTaskTemplate%2FCreate", _driver.Url);
        }

        /// <summary>
        /// Tests the return-Button
        /// </summary>
        /// <author>Matthias Grafberger</author>
        [Fact]
        public void Back()
        {
            Create_Login_Correct();

            Actions actions = new Actions(_driver);
            actions.MoveToElement(_driver.FindElement(By.Id("Back"))).Build().Perform();
            _driver.FindElement(By.Id("Back")).Click();

            Assert.Equal("https://localhost:7102/TaskTemplate/Index", _driver.Url);

        }

        /// <summary>
        /// Tests a failed creation of <see cref="TaskTemplate"> with false name and process
        /// </summary>
        /// <author>Matthias Grafberger</author>
        [Fact]
        public void Create_False_Name_And_False_Process()
        {
            Create_Login_Correct();

            Actions actions = new Actions(_driver);

            actions.MoveToElement(_driver.FindElement(By.Id("Create"))).Build().Perform();
            _driver.FindElement(By.Id("Create")).Click();

            Assert.Equal("https://localhost:7102/TaskTemplate/Create", _driver.Url);

            Assert.Contains("Ein Name muss angegeben werden", _page.NameErrorMessage);
            Assert.Contains("Es muss ein zugehöriger Prozess ausgewählt werden", _page.ProcessErrorMessage);

        }

        /// <summary>
        /// Tests a failed creation of <see cref="TaskTemplate"> with false process
        /// </summary>
        /// <author>Matthias Grafberger</author>
        [Fact]
        public void Create_False_Process()
        {
            Create_Login_Correct();

            _page.PopulateName("Test");

            Actions actions = new Actions(_driver);

            actions.MoveToElement(_driver.FindElement(By.Id("Create"))).Build().Perform();
            _driver.FindElement(By.Id("Create")).Click();

            Assert.Equal("https://localhost:7102/TaskTemplate/Create", _driver.Url);

            Assert.DoesNotContain("Ein Name muss angegeben werden", _page.NameErrorMessage);
            Assert.Contains("Es muss ein zugehöriger Prozess ausgewählt werden", _page.ProcessErrorMessage);

        }

        /// <summary>
        /// Tests a failed creation of <see cref="TaskTemplate"> with false name
        /// </summary>
        /// <author>Matthias Grafberger</author>
        [Fact]
        public void Create_False_Name()
        {
            Actions actions = new Actions(_driver);

            Create_Login_Correct();

            actions.MoveToElement(_driver.FindElement(By.Id("MakandraProcessId")));
            var selectElement = _driver.FindElement(By.Id("MakandraProcessId"));
            var select = new SelectElement(selectElement);

            _driver.FindElement(By.CssSelector("option[value='1']"));


            select.SelectByText("Onboarding");

            IWebElement selectedOption = select.SelectedOption;

             Assert.Equal("Onboarding", selectedOption.Text);

            actions.MoveToElement(_driver.FindElement(By.Id("Create"))).Build().Perform();
            _driver.FindElement(By.Id("Create")).Click();

            Assert.Equal("https://localhost:7102/TaskTemplate/Create", _driver.Url);

            Assert.Contains("Ein Name muss angegeben werden", _page.NameErrorMessage);
            Assert.DoesNotContain("Es muss ein zugehöriger Prozess ausgewählt werden", _page.ProcessErrorMessage);

        }

        /// <summary>
        /// Tests a successful creation of a <see cref="TaskTemplate"/>
        /// </summary>
        /// <author>Matthias Grafberger</author>
        [Fact]
        public void Create_Valid_TaskTemplate_1()
        {
            Actions actions = new Actions(_driver);

            Create_Login_Correct();

             _page.PopulateName("Test");

            actions.MoveToElement(_driver.FindElement(By.Id("MakandraProcessId")));
            var selectElement = _driver.FindElement(By.Id("MakandraProcessId"));
            var select = new SelectElement(selectElement);

            _driver.FindElement(By.CssSelector("option[value='1']"));

            select.SelectByText("Onboarding");

            IWebElement selectedOption = select.SelectedOption;

             Assert.Equal("Onboarding", selectedOption.Text);

            actions.MoveToElement(_driver.FindElement(By.Id("Create"))).Build().Perform();
            _driver.FindElement(By.Id("Create")).Click();

            Assert.Equal("https://localhost:7102/TaskTemplate/Index", _driver.Url);
        }

        /// <summary>
        /// Tests a successful creation of a <see cref="TaskTemplate"/>
        /// </summary>
        /// <author>Matthias Grafberger</author>
        [Fact]
        public void Create_Valid_TaskTemplate_2()
        {
            Actions actions = new Actions(_driver);

            Create_Login_Correct();

             _page.PopulateName("Test");

            actions.MoveToElement(_driver.FindElement(By.Id("MakandraProcessId")));
            var selectElement = _driver.FindElement(By.Id("MakandraProcessId"));
            var select = new SelectElement(selectElement);

            _driver.FindElement(By.CssSelector("option[value='1']"));

            select.SelectByText("Onboarding");

            IWebElement selectedOption = select.SelectedOption;

             Assert.Equal("Onboarding", selectedOption.Text);

            actions.MoveToElement(_driver.FindElement(By.Id("DefaultResponsible")));
            selectElement = _driver.FindElement(By.Id("DefaultResponsible"));
            select = new SelectElement(selectElement);

            _driver.FindElement(By.CssSelector("option[value='Vorgangsverantwortlicher']"));
            _driver.FindElement(By.CssSelector("option[value='Bezugsperson']"));

            select.SelectByText("Bezugsperson");

            selectedOption = select.SelectedOption;

             Assert.Equal("Bezugsperson", selectedOption.Text);

            actions.MoveToElement(_driver.FindElement(By.Id("Create"))).Build().Perform();
            _driver.FindElement(By.Id("Create")).Click();

            Assert.Equal("https://localhost:7102/TaskTemplate/Index", _driver.Url);
        }

        /// <summary>
        /// Tests a successful creation of a <see cref="TaskTemplate"/>
        /// </summary>
        /// <author>Matthias Grafberger</author>
        [Fact]
        public void Create_Valid_TaskTemplate_3()
        {
            Actions actions = new Actions(_driver);

            Create_Login_Correct();

             _page.PopulateName("Test");
             _page.PopulateInstruction("TestInstruction");

            actions.MoveToElement(_driver.FindElement(By.Id("MakandraProcessId")));
            var selectElement = _driver.FindElement(By.Id("MakandraProcessId"));
            var select = new SelectElement(selectElement);

            _driver.FindElement(By.CssSelector("option[value='1']"));

            select.SelectByText("Onboarding");

            IWebElement selectedOption = select.SelectedOption;

             Assert.Equal("Onboarding", selectedOption.Text);

            actions.MoveToElement(_driver.FindElement(By.Id("DefaultResponsible")));
            selectElement = _driver.FindElement(By.Id("DefaultResponsible"));
            select = new SelectElement(selectElement);

            _driver.FindElement(By.CssSelector("option[value='Vorgangsverantwortlicher']"));
            _driver.FindElement(By.CssSelector("option[value='Bezugsperson']"));
            _driver.FindElement(By.CssSelector("option[value='Administrator']"));

            select.SelectByText("Administrator");

            selectedOption = select.SelectedOption;

             Assert.Equal("Administrator", selectedOption.Text);

            actions.MoveToElement(_driver.FindElement(By.Id("Create"))).Build().Perform();
            _driver.FindElement(By.Id("Create")).Click();

            Assert.Equal("https://localhost:7102/TaskTemplate/Index", _driver.Url);
        }

        /// <summary>
        /// Tests a successful creation of a <see cref="TaskTemplate"/>
        /// </summary>
        /// <author>Matthias Grafberger</author>
        [Fact]
        public void Create_Valid_TaskTemplate_4()
        {
            Actions actions = new Actions(_driver);

            Create_Login_Correct();

             _page.PopulateName("Test");
             _page.PopulateInstruction("TestInstruction");

            actions.MoveToElement(_driver.FindElement(By.Id("MakandraProcessId")));
            var selectElement = _driver.FindElement(By.Id("MakandraProcessId"));
            var select = new SelectElement(selectElement);

            _driver.FindElement(By.CssSelector("option[value='1']"));

            select.SelectByText("Onboarding");

            IWebElement selectedOption = select.SelectedOption;

             Assert.Equal("Onboarding", selectedOption.Text);

            actions.MoveToElement(_driver.FindElement(By.Id("DefaultResponsible")));
            selectElement = _driver.FindElement(By.Id("DefaultResponsible"));
            select = new SelectElement(selectElement);

            _driver.FindElement(By.CssSelector("option[value='Vorgangsverantwortlicher']"));
            _driver.FindElement(By.CssSelector("option[value='Bezugsperson']"));
            _driver.FindElement(By.CssSelector("option[value='Administrator']"));

            select.SelectByText("Administrator");

            selectedOption = select.SelectedOption;

             Assert.Equal("Administrator", selectedOption.Text);

             actions.MoveToElement(_driver.FindElement(By.Id("Duedate")));
             selectElement = _driver.FindElement(By.Id("Duedate"));
             select = new SelectElement(selectElement);

             _driver.FindElement(By.CssSelector("option[value='2']"));

             select.SelectByText("2 Monate vor Antritt");

            selectedOption = select.SelectedOption;

             Assert.Equal("2 Monate vor Antritt", selectedOption.Text);

            actions.MoveToElement(_driver.FindElement(By.Id("Create"))).Build().Perform();
            _driver.FindElement(By.Id("Create")).Click();

            Assert.Equal("https://localhost:7102/TaskTemplate/Index", _driver.Url);
        }
        
    }
}