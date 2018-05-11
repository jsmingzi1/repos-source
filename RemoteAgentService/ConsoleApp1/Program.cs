using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServiceLibrary1;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Uri address = new Uri("http://localhost:8733/Design_Time_Addresses/WcfServiceLibrary1/Service1/mex");
            //ServiceHost selfHost = new ServiceHost(typeof(Service1), address);
            ServiceHost selfHost = new ServiceHost(typeof(Service1));
            try
            {
                //selfHost.AddServiceEndpoint(typeof(IService1), new WSHttpBinding(), address);
                //selfHost.Open();
                Console.WriteLine("success");
                Console.Read();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("Error : {0}", ce.Message);
                selfHost.Abort();
            }
            selfHost.Close();
        }
    }
}
