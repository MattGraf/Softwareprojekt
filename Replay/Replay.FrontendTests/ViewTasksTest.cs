using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace Replay.FrontendTests
{
    /// <summary>
    /// Tests to check if 
    /// </summary>
    public class ViewTasksTest : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly ViewTasksPage _page;

        public ViewTasksTest()
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
            _page = new ViewTasksPage(_driver);
            _page.Navigate();
        }
        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        [Fact]
        public void Create_ReturnIndexView()
        {
            Assert.Equal("https://localhost:7102/Account/Login?ReturnUrl=%2FMakandraTask%2FIndex", _driver.Url);
        }

        [Fact]
        public void Create_Login_Correct()
        {
            Assert.Equal("https://localhost:7102/Account/Login?ReturnUrl=%2FMakandraTask%2FIndex", _driver.Url);
            _page.PopulateLoginEmail("koenig@replay.de");
            _page.PopulateLoginPassword("GHLutz13!");
            _page.ClickLogin();
            Assert.Equal("https://localhost:7102/MakandraTask/Index", _driver.Url);
        }

        [Fact]
        public void Create_CheckIndex()
        {
            // Login correctly to pass Login screen and be logged in as Admin
            Create_Login_Correct();
            _page.CheckProcessResponsibleTable();
        }


    }
}
