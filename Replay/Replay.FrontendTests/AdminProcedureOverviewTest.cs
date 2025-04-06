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
    /// /// Test to check if the Admin Procedure Overview page is accessible
    /// /// </summary>
    /// /// <author>Arian Scheremet, Felix Nebel</author>
    public class AdminProcedureOverviewTest : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly AdminProcedureOverviewPage _page;

        public AdminProcedureOverviewTest()
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
            _page = new AdminProcedureOverviewPage(_driver);
            _page.Navigate();
        }
        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
        /// <summary>
        /// Tests if the Admin Procedure Overview Page can be correctly reached
        /// </summary>
        /// <author>Arian Scheremet</author>
        [Fact]
        public void Login_Overview_Correct()
        {
            Assert.Equal("https://localhost:7102/Account/Login?ReturnUrl=%2FProcedure%2FAdminIndex", _driver.Url);
            _page.PopulateLoginEmail("admin@replay.de");
            _page.PopulateLoginPassword("Admin1!");
            _page.ClickLogin();
            Assert.Equal("https://localhost:7102/Procedure/AdminIndex", _driver.Url);
        }
    }
}