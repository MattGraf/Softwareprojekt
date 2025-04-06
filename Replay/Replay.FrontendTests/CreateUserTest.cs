using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace Replay.FrontendTests
{
    /// <summary>
    /// Contains methods to test the creation of a <see cref="User"/>
    /// Checks if a user with correct details logs in,
    /// checks if a user with incorrect details can be created,
    /// checks if a user with correct details can be created
    /// </summary>
    /// <author>Thomas Dworschak, Felix Nebel</author>
    public class CreateUserTest : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly CreateUserPage _page;

        public CreateUserTest()
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

            _page = new CreateUserPage(_driver);
            _page.Navigate();
        }

        /// <summary>
        /// Navigates to register new <see cref="User"/>
        /// page and checks if a login is required,
        /// </summary>
        /// <author>Thomas Dworshak</author>
        [Fact]
        public void Create_ReturnCreateView()
        {
            Assert.Equal("https://localhost:7102/Account/Login?ReturnUrl=%2FAccount%2FRegister", _driver.Url);
        }

        /// <summary>
        /// Checks, if an existing <see cref="User"/>
        /// with administrator rights can log in.
        /// </summary>
        /// <author>Thomas Dworschak</author>
        [Fact]
        public void Create_Login_Correct()
        {
            Assert.Equal("https://localhost:7102/Account/Login?ReturnUrl=%2FAccount%2FRegister", _driver.Url);
            _page.PopulateLoginEmail("admin@replay.de");
            _page.PopulateLoginPassword("Admin1!");
            _page.ClickLogin();
            Assert.Equal("https://localhost:7102/Account/Register", _driver.Url);
        }

        /// <summary>
        /// Enters correct E-mail and password, but leaves
        /// required field for full name empty.
        /// Checks if <see cref="User"/> creation is
        /// rejected
        /// </summary>
        /// <author>Thomas Dworschak</author
        [Fact]
        public void Create_False_FullName()
        {
            Create_Login_Correct();
            _page.PopulateEmail("hansdieter@makandra.de");
            _page.PopulatePassword("1234");
            _page.ClickCreate();

            Assert.Equal("Der Anzeigename darf nicht leer sein.", _page.FullNameErrorMessage);
            Assert.Equal("https://localhost:7102/Account/Register", _driver.Url);
        }

        /// <summary>
        /// Enters correct full name and E-Mail, but
        /// leaves the password field empty.
        /// Checks if <see cref="User"/> creation is
        /// rejected
        /// </summary>
        /// <author>Thomas Dworschak</author>
        [Fact]
        public void Create_False_Password()
        {
            Create_Login_Correct();
            _page.PopulateFullName("Test");
            _page.PopulateEmail("hansdieter@makandra.de");
            _page.PopulatePassword("");
            _page.ClickCreate();

            Assert.Equal("Das Passwort darf nicht leer sein.", _page.PasswordErrorMessage);
            Assert.Equal("https://localhost:7102/Account/Register", _driver.Url);
        }

        /// <summary>
        /// Enters correct full name and password, but
        /// leaves the E-Mail field empty.
        /// Checks if <see cref="User"/> creation is
        /// rejected
        /// </summary>
        /// <author>Thomas Dworschak</author>
        [Fact]
        public void Create_False_Email()
        {
            Create_Login_Correct();
            _page.PopulateFullName("Test");
            _page.PopulatePassword("1234");
            _page.ClickCreate();

            Assert.Equal("Es muss eine korrekte E-Mail angegeben werden.", _page.EmailErrorMessage);
            Assert.Equal("https://localhost:7102/Account/Register", _driver.Url);
        }

        /// <summary>
        /// Enters all data correctly and checks
        /// if the <see cref="Account.Index"/> view
        /// contains the created user
        /// </summary>
        /// </author>Thomas Dworschak</author>
        [Fact]
        public void Create_WhenSuccessfullyExecuted_ReturnsIndexViewWithNewUser()
        {
            Create_Login_Correct();
            _page.PopulateFullName("TestUser");
            _page.PopulateEmail("hansdieter@makandra.de");
            _page.PopulatePassword("1234");
            _page.ClickCreate();

            Assert.Equal("https://localhost:7102/Account/Index", _driver.Url);

            Assert.Contains("Test", _page.Source);
            Assert.Contains("hansdieter@makandra.de", _page.Source);
        }
        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }     
    }
}