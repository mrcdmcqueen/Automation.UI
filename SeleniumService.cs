using OpenQA.Selenium;
using System;
using System.Threading;
using System.Xml;

namespace Mazda.Intranet.Selenium.Tests
{
    public class SeleniumService
    {
        IWebDriver driver;      
        XmlDocument elements = new XmlDocument();
        XmlDocument testElements = new XmlDocument();
        int instanceCount = 0;

        public SeleniumService()
        {
            instanceCount++;
        }
        public SeleniumService(IWebDriver _driver)
        {
            driver = _driver;
        }
        public IWebDriver Driver { get { return driver; } set { driver = value; } }

        public bool Navigate(string url, int waitTime = 5000)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(url);
            Thread.Sleep(waitTime);
            return true;
        }
        public bool Login(string user, string password, int waitTime = 10000)
        {
            while(true)
            {
                if (driver.FindElement(By.Name("username")).Displayed)
                    break;
            }

            var usernameTextbox = driver.FindElement(By.Name("username"));
            var passwordTextbox = driver.FindElement(By.Name("password"));
            var submitButton = driver.FindElement(By.Name("SUBMIT"));

            usernameTextbox.SendKeys(user);
            Thread.Sleep(1500);
            passwordTextbox.SendKeys(password);
            Thread.Sleep(2500);
            submitButton.Click();
            Thread.Sleep(waitTime);

            return true;
        }
        private XmlNode GetElementNode(string elementName, PageElementsManager pageElements)
        {
            return pageElements.GetElement(elementName);
        }
        private IWebElement GetWebElement(XmlNode pageElement)
        {
            IWebElement foundElement = null;

            var lookupValue = pageElement.Attributes["value"].Value;

            if (pageElement.Attributes["index"].Value != "*")
            {
                var maxIndex = Convert.ToInt32(pageElement.Attributes["index"].Value);
                var randomIndex = RandomNumber(1, maxIndex);

                lookupValue = lookupValue.Replace("[index]", $"[{randomIndex}]");
            }

            if (pageElement.Attributes != null)
            {
                if (pageElement.Attributes["by"].Value == "css")
                {
                    while (true)
                    {
                        if (driver.FindElement(By.CssSelector(lookupValue)).Displayed)
                            break;
                    }

                    foundElement = driver.FindElement(By.CssSelector(lookupValue));
                }
                else if (pageElement.Attributes["by"].Value == "xpath")
                {
                    while (true)
                    {
                        if (driver.FindElement(By.XPath(lookupValue)).Displayed)
                            break;
                    }
                    foundElement = driver.FindElement(By.XPath(lookupValue));
                }

                return foundElement;
            }

            return null;
        }               
        public bool Click(XmlNode node, PageElementsManager pageElements)
        {            
            var elementName = node.Attributes["element"].Value;
            
            if(pageElements != null)
            {
                var pageElement = GetElementNode(elementName, pageElements);

                if (pageElement == null)
                    return false;

                var waitTime = pageElement.Attributes["wait"] != null ? pageElement.Attributes["wait"].Value : "5000";

                if (pageElement == null)
                    return false;

                var foundElement = GetWebElement(pageElement);

                if (foundElement != null)
                    foundElement.Click();

                Thread.Sleep(Convert.ToInt32(waitTime));
                return true;
            }

            return false;
        }
        public bool SendKeys(XmlNode node, PageElementsManager pageElements, string sendText)
        {          
            var elementName = node.Attributes["element"].Value;

            if (pageElements != null)
            {
                var pageElement = GetElementNode(elementName, pageElements);

                if (pageElement == null)
                    return false;

                var waitTime = pageElement.Attributes["wait"] != null ? pageElement.Attributes["wait"].Value : "2000";
                
                var foundElement = GetWebElement(pageElement);             

                if (foundElement != null)
                {
                    foundElement.SendKeys(sendText);
                }                        

                Thread.Sleep(Convert.ToInt32(waitTime));
                return true;                              
            }

            return false;
        }
        public bool Assert(XmlNode node, PageElementsManager pageElements, string assertOperator, string value, out string error)
        {
            error = string.Empty;
            var elementName = node.Attributes["element"].Value;

            if (pageElements != null)
            {
                var pageElement = GetElementNode(elementName, pageElements);

                if (pageElement == null)
                {
                    error = $"Page Element {elementName} is null.";
                    return false;
                }
                    

                var foundElement = GetWebElement(pageElement);

                if (foundElement == null)
                {
                    error = $"Web Element {elementName} is null.";
                    return false;
                }                   

                switch (assertOperator)
                {
                    case "equal":
                    case "equals":
                    case "=":
                        {
                            return foundElement.Text == value;                           
                        }
                    case "contains":
                        {
                            return foundElement.Text.Contains(value);
                        }
                    case "startswith":
                        {
                            return foundElement.Text.StartsWith(value);
                        }
                    case "endswith":
                        {
                            return foundElement.Text.EndsWith(value);
                        }
                    case "greater":
                    case "greaterthan":
                    case ">":
                        {
                            return Convert.ToInt32(foundElement.Text) > Convert.ToInt32(value);
                        }
                    case "less":
                    case "lessthan":
                    case "<":
                        {
                            return Convert.ToInt32(foundElement.Text) < Convert.ToInt32(value);
                        }
                    case "notequal":
                    case "!=":
                        {
                            return !foundElement.Text.Equals(value);
                        }
                    case "null":
                        {
                            return string.IsNullOrEmpty(value);
                        }
                }
                
                return true;
            }
            return false;
        }
        public bool GoBack(string type="button")
        {
            if(type == "button")
            {
                driver.Navigate().Back();
            }
            else
            {
                //# Close current tab
                //driver.FindElement(By.CssSelector("body")).SendKeys(Keys.Control + "W");
                var tabs = driver.WindowHandles;
                if (tabs.Count > 1)
                {
                    driver.SwitchTo().Window(tabs[1]);
                    driver.Close();
                    driver.SwitchTo().Window(tabs[0]);
                }
            }
            
            return true;
        }
        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        public bool Exit()
        {
            if(driver != null)
            {
                if(instanceCount > 0)
                {
                    driver.Close();
                    Thread.Sleep(1000);
                    driver.Dispose();
                    instanceCount--;
                }                
            }
      
            return true;
        }
    }
}
