using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace CloudStorageActivity.Azure
{
    public class AzureVM
    {
        private Microsoft.Azure.Management.Fluent.IAzure azure;
        private AzureCredentials credentials;
        protected String subscriptionId, clientsecrett,  clientId,  tenantId;
        public AzureVM(string user, string password, string clientId, string tenantId) {
            init( user,  password,  clientId,  tenantId);
        }
        private void init(string subscriptionId, string clientsecrett, string clientId, string tenantId)
        {
            this.subscriptionId = subscriptionId;
            this.clientsecrett = clientsecrett;
            this.clientId = clientId;
            this.tenantId = tenantId;
            Console.WriteLine("clientId");
            Console.WriteLine(clientId);
            Console.WriteLine("tenantId");
            Console.WriteLine(tenantId);

            credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId
                , clientsecrett, tenantId
                , AzureEnvironment.AzureGlobalCloud);
         azure =    Microsoft.Azure.Management.Fluent.Azure.Configure()
                 .Authenticate(credentials).WithSubscription(subscriptionId);
            
         foreach(ISubscription sub in azure.Subscriptions.List()) {
                Console.WriteLine(sub.DisplayName);
            }
        }
        public String GetPublicIpAdresss(string subId, string resourceGroup, string vmName)
        {
            foreach(IVirtualMachine virtualmachine in azure.VirtualMachines.List())
            {
                Console.WriteLine("Addresss");
                Console.WriteLine(virtualmachine.GetPrimaryPublicIPAddress().IPAddress);
                Console.WriteLine("Name");
                Console.WriteLine(virtualmachine.Name);
                Console.WriteLine("ResourceGroupName");
                Console.WriteLine(virtualmachine.ResourceGroupName);
                if (virtualmachine.ResourceGroupName.ToLower().Equals(resourceGroup.ToLower()) && virtualmachine.Name.ToLower().Equals(vmName.ToLower())) {
                    return virtualmachine.GetPrimaryPublicIPAddress().IPAddress;
                }
            }
           // var ipadress = azure.VirtualMachines.GetById(vmName).GetPrimaryPublicIPAddress().IPAddress;
            //Console.WriteLine("Addresss");
            //Console.WriteLine(ipadress);
           // Console.ReadLine();
            var
            credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId
                , clientsecrett, tenantId
                , AzureEnvironment.AzureGlobalCloud);
            var client = new ComputeManagementClient(credentials);
            
                client.SubscriptionId = subId;

                VirtualMachineInner vm = VirtualMachinesOperationsExtensions.GetAsync(client.VirtualMachines, resourceGroup, vmName).Result;

              var  networkName = vm.NetworkProfile.NetworkInterfaces[0].Id.Split('/').Last();

            var clientNetwork = new NetworkManagementClient(credentials);

               clientNetwork.SubscriptionId = subId;
                var network = NetworkInterfacesOperationsExtensions.GetAsync(clientNetwork.NetworkInterfaces, resourceGroup, vmName).Result;
                string ip = network.IpConfigurations[0].PrivateIPAddress;
            return ip;



        }

        public void CreateDebian8VM(string groupName, string vmName, String region, string azureuser, string adminPassword, String
            imgName = KnownLinuxImages.Debian8)
        {
            CreateUbuntu16VM(groupName, vmName, region, azureuser, adminPassword, imgName);
        }
        public void CreateCentOS7VM(string groupName, string vmName, String region, string azureuser, string adminPassword, String
            imgName = KnownLinuxImages.CentOS7)
        {
            CreateUbuntu16VM(groupName, vmName, region, azureuser, adminPassword, imgName);
        }
        public void CreateUbuntu14VM(string groupName, string vmName, String region, string azureuser, string adminPassword, String
            imgName = KnownLinuxImages.UbuntuServer14)
        {
            CreateUbuntu16VM(groupName, vmName, region, azureuser, adminPassword, imgName);
        }
        public void CreateUbuntu16VM(string groupName, string vmName,String region, string azureuser, string adminPassword, String
            imgName= KnownLinuxImages.UbuntuServer16) {
            var mKnownLinuxVirtualMachineImage = KnownLinuxImages.KnownLinuxVirtualMachineImageFromName(imgName);
            Region location = Region.Create(region);
            Console.WriteLine("Creating resource group...");
            Console.WriteLine(groupName);
            Console.WriteLine(location.Name);
            var resourceGroup = azure.ResourceGroups.Define(groupName)
                .WithRegion(location)
                .Create();

            Console.WriteLine("Creating availability set...");
            var availabilitySet = azure.AvailabilitySets.Define("myAVSet")
                .WithRegion(location)
                .WithExistingResourceGroup(groupName)
                .WithSku(AvailabilitySetSkuTypes.Managed)
                .Create();

            Console.WriteLine("Creating public IP address...");
            var publicIPAddress = azure.PublicIPAddresses.Define("myPublicIP")
                .WithRegion(location)
                .WithExistingResourceGroup(groupName)
                .WithDynamicIP()
                .Create(); Console.WriteLine("Creating virtual network...");

            var network = azure.Networks.Define("myVNet")
                .WithRegion(location)
                .WithExistingResourceGroup(groupName)
                .WithAddressSpace("10.0.0.0/16")
                .WithSubnet("mySubnet", "10.0.0.0/24")
                .Create();

            Console.WriteLine("Creating network interface...");
            var networkInterface = azure.NetworkInterfaces.Define("myNIC")
                .WithRegion(location)
                .WithExistingResourceGroup(groupName)
                .WithExistingPrimaryNetwork(network)
                .WithSubnet("mySubnet")
                .WithPrimaryPrivateIPAddressDynamic()
                .WithExistingPrimaryPublicIPAddress(publicIPAddress)
                .Create();

            Console.WriteLine("Creating virtual machine...");
            var vm = azure.VirtualMachines.Define(vmName);
            vm.WithRegion(location)
                .WithExistingResourceGroup(groupName)
                .WithExistingPrimaryNetworkInterface(networkInterface)
                .WithPopularLinuxImage(mKnownLinuxVirtualMachineImage)
                .WithRootUsername(azureuser)
                .WithRootPassword(adminPassword)
                .WithComputerName(vmName)
                .WithExistingAvailabilitySet(availabilitySet)
                .WithSize(VirtualMachineSizeTypes.StandardA1)
                .Create();
        }

        public void CreateWindowsVM(string groupName, string vmName, Region location, string azureuser, string adminPassword, KnownWindowsVirtualMachineImage
            mKnownWindowsVirtualMachineImage) {
            Console.WriteLine("Creating resource group...");
            var resourceGroup = azure.ResourceGroups.Define(groupName)
                .WithRegion(location)
                .Create();

            Console.WriteLine("Creating availability set...");
            var availabilitySet = azure.AvailabilitySets.Define("myAVSet")
                .WithRegion(location)
                .WithExistingResourceGroup(groupName)
                .WithSku(AvailabilitySetSkuTypes.Managed)
                .Create();

            Console.WriteLine("Creating public IP address...");
            var publicIPAddress = azure.PublicIPAddresses.Define("myPublicIP")
                .WithRegion(location)
                .WithExistingResourceGroup(groupName)
                .WithDynamicIP()
                .Create(); Console.WriteLine("Creating virtual network...");

            var network = azure.Networks.Define("myVNet")
                .WithRegion(location)
                .WithExistingResourceGroup(groupName)
                .WithAddressSpace("10.0.0.0/16")
                .WithSubnet("mySubnet", "10.0.0.0/24")
                .Create();

            Console.WriteLine("Creating network interface...");
            var networkInterface = azure.NetworkInterfaces.Define("myNIC")
                .WithRegion(location)
                .WithExistingResourceGroup(groupName)
                .WithExistingPrimaryNetwork(network)
                .WithSubnet("mySubnet")
                .WithPrimaryPrivateIPAddressDynamic()
                .WithExistingPrimaryPublicIPAddress(publicIPAddress)
                .Create();

            Console.WriteLine("Creating virtual machine...");
            var vm = azure.VirtualMachines.Define(vmName);
            vm.WithRegion(location)
                .WithExistingResourceGroup(groupName)
                .WithExistingPrimaryNetworkInterface(networkInterface)
                .WithPopularWindowsImage(mKnownWindowsVirtualMachineImage)
                .WithAdminUsername(azureuser)
                .WithAdminPassword(adminPassword)
                .WithComputerName(vmName)
                .WithExistingAvailabilitySet(availabilitySet)
                .WithSize(VirtualMachineSizeTypes.StandardDS1)
                .Create();
        }
        public static class KnownLinuxImages {
            public const String UbuntuServer14 = "UbuntuServer14";
            public const String UbuntuServer16 = "UbuntuServer16";
            public const String Debian8 = "Debian8";
            public const String CentOS7 = "CentOS7";

            public static String[] AllKnownLinuxImages = new String[] { UbuntuServer14, UbuntuServer16, Debian8, CentOS7 };
            public static KnownLinuxVirtualMachineImage[] AllActualKnownLinuxVirtualMachineImage = new KnownLinuxVirtualMachineImage[] {
                KnownLinuxVirtualMachineImage.UbuntuServer14_04_Lts,KnownLinuxVirtualMachineImage.UbuntuServer16_04_Lts,
                KnownLinuxVirtualMachineImage.Debian8,KnownLinuxVirtualMachineImage.CentOS7_2
            };
            public static KnownLinuxVirtualMachineImage KnownLinuxVirtualMachineImageFromName(String name)
            {
                return AllActualKnownLinuxVirtualMachineImage[AllKnownLinuxImages.ToList().IndexOf(name)];
            }
        }
        
    }
}
