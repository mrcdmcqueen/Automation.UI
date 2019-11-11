using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mazda.Intranet.Selenium.Tests.Browser
{
    public static class FirefoxService
    {
        public static FirefoxDriverService GetFirefoxService()
        {
            //Give the path of the geckodriver.exe    
            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(@"C:\Users\cmcqueen\Downloads\geckodriver-v0.26.0-win64", "geckodriver.exe");

            //Give the path of the Firefox Browser        
            service.FirefoxBinaryPath = @"C:\\Program Files\\Mozilla Firefox\\firefox.exe";

            return service;
        }
    }
}
