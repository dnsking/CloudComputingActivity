using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Compute.v1;
using Google.Apis.Compute.v1.Data;
using Google.Apis.Requests;
using Google.Apis.Services;

namespace CloudStorageActivity.GoogleCloud.CloudCompute
{
    public class CloudComputeConnector
    {
        private ComputeService service;
        public CloudComputeConnector(String email, String clientId, String clientSecret, String applicationName=null, String apiKey = null) {
            init(email, clientId,  clientSecret,  applicationName,  apiKey);
        }
        private void init(String email,String clientId,String clientSecret,String applicationName=null,String apiKey=null)
        {
            Console.WriteLine("email");
            Console.WriteLine(email);
            Console.WriteLine("clientId");
            Console.WriteLine(clientId);
            Console.WriteLine("clientSecret");
            Console.WriteLine(clientSecret);
            UserCredential credential =  GoogleWebAuthorizationBroker.AuthorizeAsync(
        new ClientSecrets
        {
            ClientId = clientId,
            ClientSecret = clientSecret
        },
         new[] { ComputeService.Scope.CloudPlatform, ComputeService.Scope.CloudPlatform },
        email,
         new CancellationTokenSource().Token).Result;

             service = new ComputeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
                ApiKey = apiKey
            });

        }
        public String GetInstanceShh(String project, String zone,String name)
        {
            String shhkey = null;
            var instance = service.Instances.Get(project, zone, name).Execute();

            Console.WriteLine(instance.SelfLink);
            
           var items = instance.Metadata;

            
            foreach (Metadata.ItemsData itemData in items.Items)
            {
                if (itemData.Key.Equals("shh-keys"))
                {
                    shhkey = itemData.Value;
                    Console.WriteLine("shhkey");
                    Console.WriteLine(shhkey);
                    Console.ReadLine();
                    return itemData.Value;
                }
            }
            return shhkey;
        }
        public Instance[] ListInstances(String project, String zone)
        {
            return service.Instances.List(project, zone).Execute().Items.ToArray();
        }
        public void InsertInstance(String zone,String name, String imgProject,String imgFamily,String interface__= "SCSI") {
            GetInstanceShh("api-project-545812698446", zone, name);
            var image = service.Images.GetFromFamily(imgProject, imgFamily).Execute();
          
            IList<AttachedDisk> attachedDisks = new List<AttachedDisk>();
            AttachedDisk attachedDisk = new AttachedDisk();
            AttachedDiskInitializeParams attachedDiskInitializeParams = new AttachedDiskInitializeParams();
            attachedDiskInitializeParams.DiskSizeGb = image.DiskSizeGb;
            attachedDiskInitializeParams.SourceImage = image.SelfLink;


            attachedDisk.AutoDelete = true;
            attachedDisk.Boot = true;
            attachedDisk.Interface__ = interface__;
            attachedDisk.InitializeParams = attachedDiskInitializeParams;
            attachedDisks.Add(attachedDisk);

            var accessConfig = new AccessConfig();
            accessConfig.Name = "External NAT";
            accessConfig.Type = "ONE_TO_ONE_NAT";

            IList<NetworkInterface> networkInterfaces = new List<NetworkInterface>();
            NetworkInterface networkInterface = new NetworkInterface();
            networkInterface.Network =  "/global/networks/default";
            networkInterface.AccessConfigs = new List<AccessConfig>() { accessConfig };
            networkInterfaces.Add(networkInterface);

            Tags tags = new Tags();
            IList<string> stringList = new List<string>();

            tags.Items = new List<string>();
            tags.Items.Add("http-server");
            tags.Items.Add("https-server");
            

            Instance instance = new Instance()
            {
                MachineType = "zones/"+zone+"/machineTypes/n1-standard-1" ,
               // Metadata = metaData,
                Name = name,
                Tags = tags,
                NetworkInterfaces = networkInterfaces,
                Disks = attachedDisks
                
            };
            service.Instances.Insert(instance, "api-project-545812698446", zone).Execute();
        }
    }
}
