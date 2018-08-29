using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudStorageActivity.Aws.Ec2;
using Renci.SshNet;

namespace CloudStorageActivity.Azure.Vm
{
    public class AzureVMTerminalRunner : AzureVM
    {
        public AzureVMTerminalRunner(string user, string password, string clientId, string tenantId) : base(user, password, clientId, tenantId)
        {
        }

        public  String[] RunUnixCommand(String resourceGroup, String vmName, String[] commands, String username ,String vmpassword)
        {
            var host = GetPublicIpAdresss(subscriptionId, resourceGroup, vmName);
            Console.WriteLine(host);
            //Console.ReadLine();
            var responses = new List<String>();
           
            using (var sshclient = new SshClient(host, username, vmpassword))
            {
                sshclient.Connect();
                foreach (String cmd in commands)
                {
                    Console.WriteLine("cmd");
                    Console.WriteLine(cmd);
                    var smmcmd = sshclient.CreateCommand(cmd);
                    smmcmd.Execute();
                    String response = smmcmd.Result;
                    Console.WriteLine("response");
                    Console.WriteLine(response);
                    responses.Add(response);
                }
                sshclient.Disconnect();
            }
            return responses.ToArray();
        }

    }
}
