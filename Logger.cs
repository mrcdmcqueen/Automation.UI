using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mazda.Intranet.Selenium.Tests
{
    public class Logger
    {
        public string filePath = Environment.CurrentDirectory + @"\TestLog.txt";
        public void Log(string message)
        {
            File.AppendAllText(filePath, message + Environment.NewLine);
            //using (StreamWriter streamWriter = new StreamWriter(filePath))
            //{
            //    streamWriter.WriteLine(message);
            //    streamWriter.Close();
            //}
        }
    }
}
