using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace CloudStorageActivity.Aws.Ec2
{
    public class EC2TerminalRunner
    {
        public static String[] RunUnixCommand(String host,String privateKeyPath,String[] commands,String username="ubuntu")
        {

            Console.WriteLine(host);
            Console.WriteLine(privateKeyPath);
            Console.WriteLine(commands[0]);
            //Console.ReadLine();
            var responses = new List<String>();
            ConnectionInfo ConnNfo = new ConnectionInfo(host, 22, username,
              new AuthenticationMethod[]{
                  
                new PrivateKeyAuthenticationMethod(username,new PrivateKeyFile[]{
                    new PrivateKeyFile(privateKeyPath)
                }),
              }
           );
            using (var sshclient = new SshClient(ConnNfo))
            {
                sshclient.Connect();
                foreach(String cmd in commands)
                {
                    Console.WriteLine("cmd");
                    Console.WriteLine(cmd);
                    String response =  sshclient.CreateCommand(cmd).Execute();
                    Console.WriteLine("response");
                    Console.WriteLine(response);
                    responses.Add(response);
                }
                sshclient.Disconnect();
            }
            return responses.ToArray();
        }
        public void RunUnixUpload(String host, String privateKeyPath,String filePath,String dir, String username = "ubuntu")
        {

            ConnectionInfo ConnNfo = new ConnectionInfo(host, 22, username,
              new AuthenticationMethod[]{ new PrivateKeyAuthenticationMethod(username,new PrivateKeyFile[]{
                    new PrivateKeyFile(privateKeyPath)
                }),
              }
           );
            using (var sftp = new SftpClient(ConnNfo))
            {

                sftp.Connect();
                sftp.ChangeDirectory(dir);
                using (var uplfileStream = System.IO.File.OpenRead(filePath))
                {
                    sftp.UploadFile(uplfileStream, filePath, true);
                }
                sftp.Disconnect();
            }
        }
    }
}
