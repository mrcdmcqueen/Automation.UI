using System;
using System.Xml;

namespace Mazda.Intranet.Selenium.Tests
{
    public class PageElementsManager
    {
        XmlDocument doc = new XmlDocument();
        public PageElementsManager(string elementsFile)
        {
            doc.Load(Environment.CurrentDirectory + $"/{elementsFile}Elements.xml");
        }

        public XmlNode GetElement(string elementName)
        {
            return doc.SelectSingleNode($"//Element[@name='{elementName}']");
        }
    }
}
