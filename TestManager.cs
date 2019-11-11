using Mazda.Intranet.Selenium.Tests.Browser;
using OpenQA.Selenium.Firefox;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace Mazda.Intranet.Selenium.Tests
{
    public class TestManager
    {
        SeleniumService seleniumService = new SeleniumService();
        Logger logger = new Logger();
        PageElementsManager elementsManager;
        string currentTestPage = string.Empty;

        public void RunTests(string path=".")
        {
            const string message = "WELCOME TO TESTMANAGER!";
            string[] files = new string[0];

            if (path.Equals("./"))
                files = Directory.GetFiles(Environment.CurrentDirectory, "*_TestManager.xml");
            else
                files = Directory.GetFiles(path, "*_TestManager.xml");

            foreach(var file in files)
            {
                logger.Log(message);
                Console.WriteLine(message);
                currentTestPage = Path.GetFileNameWithoutExtension(file).Replace("_TestManager","");
                elementsManager = new PageElementsManager(currentTestPage);
                RunTestFile(file);
            }
        }

        public void RunTestFile(string testFile)
        {
            int testIndex = 1;
            XmlDocument doc = new XmlDocument();
            doc.Load(testFile);

            var tests = doc.GetElementsByTagName("Test");

            foreach(XmlNode test in tests)
            {
                var message = $"Test {testIndex}. Name: {test.Attributes["name"].Value} starting...";
                logger.Log(message);
                Console.WriteLine(message);

                var failedCount = Convert.ToInt32(string.IsNullOrEmpty(test.Attributes["failed"].Value) ? "0" : test.Attributes["failed"].Value);
                if(failedCount < 5)
                {
                    if (!RunTestCommands(test))
                    {
                        failedCount++;
                        test.Attributes["failed"].Value = failedCount.ToString();
                        doc.Save(testFile);
                        message = $"Test {testIndex} failed! Name: {test.Attributes["name"].Value}";
                        logger.Log(message);
                        Console.WriteLine(message);
                    }
                    else
                    {
                        message = $"Test {testIndex} completed! Name: {test.Attributes["name"].Value}";
                        logger.Log(message);
                        Console.WriteLine(message);
                    }

                    testIndex++;
                }               
            }
        }

        public bool RunTestCommands(XmlNode test)
        {
            string commandName = string.Empty;

            if (test.HasChildNodes)
            {
                var commands = test.SelectSingleNode("//Commands");

                if (null == commands.FirstChild)
                    return false;

                if (!RunTestCommand1(commands.FirstChild))
                    return false;

                foreach (XmlNode command in commands)
                { 
                    if (command.Attributes["name"] != null && command.Attributes["name"].Value != "command1")
                    {
                        commandName = command.Attributes["name"].Value;                       

                        try
                        {
                            switch (command.Attributes["action"].Value)
                            {
                                case "click":
                                    {
                                        seleniumService.Click(command, elementsManager);
                                       
                                        break;
                                    }
                                case "verify":
                                    {
                                        string error = string.Empty;
                                        if( !seleniumService.Assert(command, elementsManager, command.Attributes["verify"].Value, 
                                            command.Attributes["verifyValue"].Value, out error))
                                        {
                                            logger.Log(error);
                                            return false;
                                        }
                                        break;                                        
                                    }
                                case "close":
                                    {
                                        seleniumService.Exit();
                                        break;
                                    }
                                 case "back":
                                    {                                       

                                        seleniumService.GoBack();

                                        break;
                                    }
                                case "tab_back":
                                    {                                        
                                        seleniumService.GoBack("tab");

                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                        catch(Exception)
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.AppendLine($"   Command {command.Attributes["name"].Value} failed on test [{test.Attributes["name"].Value}]!");
                            builder.AppendLine($"   action:{command.Attributes["action"].Value}  element:{command.Attributes["element"].Value}  assert:{command.Attributes["assert"].Value}");

                            if (command.Attributes["assert"].Value.ToLower() == "true")
                            {
                                builder.AppendLine($"verify:{command.Attributes["verify"].Value}  verifyElement:{command.Attributes["verifyElement"].Value}  verifyValue:{command.Attributes["verifyValue"].Value}");
                            }

                            logger.Log(builder.ToString());
                            return false;
                        }
                    }                  
                }
                return false;
            }

            return false;
        }

        public void TestCommandFailed()
        {

        }

        public bool RunTestCommand1(XmlNode command1)
        {
            GetBrowserConfiguration();
            seleniumService.Navigate(command1.Attributes["url"].Value);
            //Thread.Sleep(Convert.ToInt32(command1.Attributes["wait"].Value));   
            
            if(Convert.ToBoolean( command1.Attributes["authenticate"].Value))
            {
                seleniumService.Login(command1.Attributes["user"].Value, command1.Attributes["password"].Value);
            }
            
            return true;           
        }

        public void GetBrowserConfiguration()
        {
            var browser = string.IsNullOrEmpty(ConfigurationManager.AppSettings["browser"]) ? "firefox" : ConfigurationManager.AppSettings["browser"].ToString();

            switch (browser)
            {
                case "firefox":
                    {
                        var service = FirefoxService.GetFirefoxService();
                        seleniumService.Driver = new FirefoxDriver(service);
                        break;
                    }
                default:
                    {
                        var service = FirefoxService.GetFirefoxService();
                        seleniumService.Driver = new FirefoxDriver(service);
                        break;
                    }
            }            
        }

        public bool ExecutTestCommand(XmlNode command)
        {
            return false;
        }

        public void Close()
        {
            seleniumService.Exit();
        }
    }
}
