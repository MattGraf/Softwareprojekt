using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace Replay.FrontendTests.Process
{
    public class CreateProcessTest : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly CreateProcessPage _page;

        public CreateProcessTest()
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
            _page = new CreateProcessPage(_driver);
            _page.Navigate();
        }
        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
        [Fact]
        public void Create_Login_Correct()
        {
            Assert.Equal("https://localhost:7102/Account/Login?ReturnUrl=%2FMakandraProcess%2FCreate", _driver.Url);
            _page.PopulateLoginEmail("admin@replay.de");
            _page.PopulateLoginPassword("Admin1!");
            _page.ClickLogin();
            Assert.Equal("https://localhost:7102/MakandraProcess/Create", _driver.Url);
        }


        [Fact]
        public void Create_ReturnCreateView()
        {
            Assert.Equal("https://localhost:7102/Account/Login?ReturnUrl=%2FMakandraProcess%2FCreate", _driver.Url);
        }

        [Fact]
        public void Create_False_Descriptor()
        {
            Create_Login_Correct();
            _page.ClickCreate();

            Assert.Equal("Der Bezeichner darf nicht leer sein.", _page.DescriptorErrorMessage);
            Assert.Equal("https://localhost:7102/MakandraProcess/Create", _driver.Url);
        }
        [Fact]
        public void Create_WhenSuccessfullyExecuted_ReturnsIndexViewWithNewUser()
        {
            Create_Login_Correct();
            _page.PopulateDescriptor("TestProcess");
            _page.ClickCreate();

            Assert.Equal("https://localhost:7102/MakandraProcess/Index", _driver.Url);

            Assert.Contains("TestProcess", _page.Source);
        }

    }
}
