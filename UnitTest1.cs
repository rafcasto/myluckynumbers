using DotNetCoreSelenium.Helpers;
using DotNetCoreSelenium.Models;
using DotNetCoreSelenium.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenQA;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Threading;

namespace DotNetCoreSelenium
{
    [TestClass]
    public class UnitTest1
    {
        private LottoHelper _lottoHelper;

        private IWebDriver driver;
        public UnitTest1()
        {
            _lottoHelper = new LottoHelper();

            

        }
        [TestMethod]
        public void TestMethod1()
        {
          
           var WinnerNumbers = _lottoHelper.GetListOfMyNextWinningNumbers(true);
             driver = new ChromeDriver(); 
            driver.Navigate().GoToUrl("https://mylotto.co.nz/lotto");
           
          
         //   driver.FindElement(By.XPath("//*[@id='loginForm']/div[1]/div[1]")).Click();
         
         
           
           
           driver.FindElement(By.Id("pickYourOwn")).Click(); 
           driver.FindElement(By.Id("powerballToggle")).Click();

           foreach(var winnerNumber in WinnerNumbers){
               int counter = 0;
               foreach(var win in winnerNumber.Value){
                   if(counter > 8){
                       break;
                   }
                   SelectNumbers(win.LotoNumber);
                   counter++;
               }
           } 

//           SendKeys(email,"rafcasto@gmail.com");
  //         Tab(email);
    //       SendKeys(password,"rafael88");
          
         //   driver.FindElement(By.Id("password")).Click();
            
              //var email = driver.FindElement(By.Name("emailAddress"));
            //var password = driver.FindElement(By.Name("password"));
              //var loginButton = driver.FindElement(By.Id("login-submit-button"));
            
// var menuLink = driver.FindElement(By.Id("nav-menu-account--link"));
         //   menuLink.Click();
           // var currentWindown = driver.CurrentWindowHandle;
           // driver.FindElement(By.Id("rightMenuNavMenuLoginBlockLoginButton")).Click();
         
          //  driver.Close();
         
           // driver.FindElement(By.Id("buyNowButtonOuter")).Click();
        //    driver.FindElement(By.Id("confirmation-button")).Click();
        }

        private void SendKeys(IWebElement element, string text){
            Actions action = new Actions(driver);
            action.MoveByOffset(element.Location.X,element.Location.Y).Perform();
            action.MoveToElement(element).SendKeys(text).Perform();
        }

        private void Tab(IWebElement element){
            Actions action = new Actions(driver);
            action.MoveToElement(element).KeyDown(Keys.Tab).KeyUp(Keys.Tab).Build().Perform();
        }

        private void SelectNumbers(string[] number){
            foreach(var lotoNumber in number){
                var myLuckNumber = $"lottoNumber{int.Parse(lotoNumber)}";
                var lottoNumberSelector = driver.FindElement(By.Id("lottoNumberSelector"));
            
                lottoNumberSelector.FindElement(By.Id(myLuckNumber)).Click(); 
            }
        }
    }
}
