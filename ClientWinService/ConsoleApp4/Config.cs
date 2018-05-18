using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Config
    {
        Dictionary<string, string> properDic = new Dictionary<string, string>();
        public bool load(string filename)
        {
            if (File.Exists(filename) == false)
                return false;
            foreach (var row in File.ReadAllLines(filename))
            {
                if (row.Contains("="))
                    properDic.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
            }
            return true;
        }
        public string getValue(string propertyname)
        {
            return properDic[propertyname];
        }
        public bool isExist(string propertyname)
        {
            return properDic.Keys.Contains(propertyname);
        }
        //BIOS Identifier
        public static string cpuid()
        {
            ManagementClass managClass = new ManagementClass("win32_processor");
            ManagementObjectCollection managCollec = managClass.GetInstances();

            foreach (ManagementObject managObj in managCollec)
            {
                return managObj.Properties["processorID"].Value.ToString();
            }
            return "1234";
        }
    }
}
