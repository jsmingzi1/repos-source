using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2");

            string queryString = "select * from win32_logonsession";                          // for all sessions
            //string queryString = "select * from win32_logonsession where logontype = 2";     // for local interactive sessions only

            ManagementObjectSearcher sessionQuery = new ManagementObjectSearcher(scope, new SelectQuery(queryString));
            ManagementObjectCollection logonSessions = sessionQuery.Get();
            foreach (ManagementObject logonSession in logonSessions)
            {
                string logonID = logonSession["LogonId"].ToString();
                string status = "";// logonSession["Status"].ToString();
                Console.WriteLine("=== {0}, type {1} === statu {2}", logonID, logonSession["LogonType"].ToString(), status);
                RelatedObjectQuery relatedQuery = new RelatedObjectQuery("associators of {Win32_LogonSession.LogonId='" + logonID + "'} WHERE AssocClass=Win32_LoggedOnUser Role=Dependent");
                ManagementObjectSearcher userQuery = new ManagementObjectSearcher(scope, relatedQuery);
                ManagementObjectCollection users = userQuery.Get();
                foreach (ManagementObject user in users)
                {
                    Console.WriteLine(user.GetPropertyValue("Name"));
                    Console.WriteLine(user.GetPropertyValue("Status"));
                }
            }
            Console.Read();
        }
    }
}
