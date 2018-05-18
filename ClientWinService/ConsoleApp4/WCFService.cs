using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    //BELOW IS WCF DEFINITION

    [ServiceContract(Namespace = "http://www.khita.cn.samples")]
    public interface IWinStation
    {
        [OperationContract]
        void UnLockWinStation(string username, string password, string other, int type);
    }

    public class WinStationService : IWinStation
    {
        public void UnLockWinStation(string username, string password, string other, int type)
        {
            //string User = Win32Sessions.GetActiveConsoleUserWithDomain();
            //using (StreamWriter sw = File.AppendText("c:\\2\\run.log"))
            //{
            //    sw.WriteLine(DateTime.Now.ToString()+":"+ User);
            //}
            if (type != 0 && (string.IsNullOrEmpty(username)||string.IsNullOrEmpty(password)))
            {
                //only type 0, support empty credential, and use default one, others won't, directly return
                return;
            }
            if (ClientWinService.m_SessionState != SessionChangeReason.SessionUnlock && ClientWinService.m_SessionState != SessionChangeReason.SessionLogon && ClientWinService.m_SessionState != SessionChangeReason.RemoteConnect)
            {
                Win32Helper.Win7AboveSendSAS(false);
                if (type == 0)
                    Utils.UnLockWinStation("", "", "", type);
                else
                    Utils.SendUserNameAndPassword(username, password);
            }
        }
    }

}
