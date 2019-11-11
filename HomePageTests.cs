using NUnit.Framework;
using System;
using System.Diagnostics;

namespace Mazda.Intranet.Selenium.Tests
{
    public class HomePageTests
    {
        [SetUp]
        public void startBrowser()
        {            
        }

        [Test]
        public void test()
        {  
            try
            {
                TestManager manager = new TestManager();
                manager.RunTests();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
           
           
        }

        [TearDown]
        public void closeBrowser()
        {
            //driver.Close();
        }

    }
}
