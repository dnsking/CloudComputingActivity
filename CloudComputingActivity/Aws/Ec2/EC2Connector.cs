using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;

namespace CloudStorageActivity.Aws.Ec2
{
    public class EC2Connector
    {
        private AmazonEC2Client ec2Client;
        private const String DefaultSecurityGroup = "launch-wizard-1";
        public EC2Connector(String myaccesskey, String mysecretkey, String region)
        {
            var awsCredentials = new BasicAWSCredentials(myaccesskey, mysecretkey);
            ec2Client = new AmazonEC2Client(awsCredentials, Regions.RegionEndpointFromName(region));
        }
        public List<KeyPairInfo> ListKeyPairs()
        {
            var request = new DescribeKeyPairsRequest();
            var response = ec2Client.DescribeKeyPairs(request);
            return response.KeyPairs;
        }
        public void DeleteKeyPair(KeyPair keyPair) { ec2Client.DeleteKeyPair(new DeleteKeyPairRequest { KeyName = keyPair.KeyName }); }
        public void CreateKeyPair(string keyPairName,string privateKeyFile)
        {
            var request = new CreateKeyPairRequest();
            request.KeyName = keyPairName;
            var response = ec2Client.CreateKeyPair(request);

            using (FileStream s = new FileStream(privateKeyFile, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(s))
            {
                writer.WriteLine(response.KeyPair.KeyMaterial);
            }
        }
        public void AddRuleToEc2SecurityGroup(String ipRange = "1.1.1.1/1", String ipProtocol = "tcp", int fromPort= 3389, int toPort= 3389
            , string secGroupName = DefaultSecurityGroup) {
            List<string> ranges = new List<string>() { ipRange };

            var ipPermission = new IpPermission();
            ipPermission.IpProtocol = ipProtocol;
            ipPermission.FromPort = fromPort;
            ipPermission.ToPort = toPort;
            ipPermission.IpRanges = ranges;
            var ingressRequest = new AuthorizeSecurityGroupIngressRequest();
            ingressRequest.GroupId = secGroupName;
            ingressRequest.IpPermissions.Add(ipPermission);
            var ingressResponse = ec2Client.AuthorizeSecurityGroupIngress(ingressRequest);
        }
        public SecurityGroup Ec2SecurityGroup(string secGroupName= DefaultSecurityGroup)
        {
            Filter nameFilter = new Filter();
            nameFilter.Name = "group-name";
            nameFilter.Values = new List<string>() { secGroupName };

            var describeRequest = new DescribeSecurityGroupsRequest();
            describeRequest.Filters.Add(nameFilter);
            var describeResponse = ec2Client.DescribeSecurityGroups(describeRequest);
            
            if (describeResponse.SecurityGroups.Count > 0)
            {
                return describeResponse.SecurityGroups[0];
            }

            // Create the security group
            var createRequest = new CreateSecurityGroupRequest();
            createRequest.GroupName = secGroupName;
            createRequest.Description = "My sample security group for EC2-Classic";

            var createResponse = ec2Client.CreateSecurityGroup(createRequest);

            var Groups = new List<string>() { createResponse.GroupId };
            describeRequest = new DescribeSecurityGroupsRequest() { GroupIds = Groups };
            describeResponse = ec2Client.DescribeSecurityGroups(describeRequest);
            return describeResponse.SecurityGroups[0];
        }
        public List<SecurityGroup> SecurityGroupsAvaliable()
        {
            var request = new DescribeSecurityGroupsRequest();
            var response = ec2Client.DescribeSecurityGroups(request);
            List<SecurityGroup> mySGs = response.SecurityGroups;
            return mySGs;
        }
        public String[] CreateInstance(string amiID, string keyPairName,string instanceType,String securityGroupId= DefaultSecurityGroup,String keypath=null)
        {
            if (keypath != null)
                CreateKeyPair(keyPairName, keypath);

            List<string> groups = new List<string>() { securityGroupId };
            var launchRequest = new RunInstancesRequest()
            {
                ImageId = amiID,
                InstanceType = Instances.InstanceTypeFromName(instanceType),
                MinCount = 1,
                MaxCount = 1,
                KeyName = keyPairName,
                SecurityGroupIds = groups
            };
            var launchResponse = ec2Client.RunInstances(launchRequest);
            var instances = launchResponse.Reservation.Instances;
            var instanceIds = new List<string>();
            foreach (Instance item in instances)
            {
                instanceIds.Add(item.InstanceId);
                Console.WriteLine();
                Console.WriteLine("New instance: " + item.InstanceId);
                Console.WriteLine("Instance state: " + item.State.Name);
            }
            return instanceIds.ToArray();
            }
        public void StopInstance(string InstanceID)
        {
            var mStopInstancesRequest = new StopInstancesRequest();
            mStopInstancesRequest.InstanceIds = new List<string> { InstanceID };
            ec2Client.StopInstances(mStopInstancesRequest);
        }

        public void StartInstance(string InstanceID)
        {
               var mStartInstancesRequest = new StartInstancesRequest();
            mStartInstancesRequest.InstanceIds = new List<string> { InstanceID };
            ec2Client.StartInstances(mStartInstancesRequest);
        }
        public  void TerminateInstance(string instanceId)
        {
            var request = new TerminateInstancesRequest();
            request.InstanceIds = new List<string>() { instanceId };

            try
            {
                var response = ec2Client.TerminateInstances(request);
                foreach (InstanceStateChange item in response.TerminatingInstances)
                {
                    Console.WriteLine("Terminated instance: " + item.InstanceId);
                    Console.WriteLine("Instance state: " + item.CurrentState.Name);
                }
            }
            catch (AmazonEC2Exception ex)
            {
                
            }
        }
        public static class Instances
        {
                public static string  C1Medium       ="C1Medium";
                public static string  C1Xlarge       ="C1Xlarge";
                public static string  C32xlarge      ="C32xlarge";
                public static string  C34xlarge      ="C34xlarge";
                public static string  C38xlarge      ="C38xlarge";
                public static string  C3Large        ="C3Large";
                public static string  C3Xlarge       ="C3Xlarge";
                public static string  C42xlarge      ="C42xlarge";
                public static string  C44xlarge      ="C44xlarge";
                public static string  C48xlarge      ="C48xlarge";
                public static string  C4Large        ="C4Large";
                public static string  C4Xlarge       ="C4Xlarge";
                public static string  C518xlarge     ="C518xlarge";
                public static string  C52xlarge      ="C52xlarge";
                public static string  C54xlarge      ="C54xlarge";
                public static string  C59xlarge      ="C59xlarge";
                public static string  C5d18xlarge    ="C5d18xlarge";
                public static string  C5d2xlarge     ="C5d2xlarge";
                public static string  C5d4xlarge     ="C5d4xlarge";
                public static string  C5d9xlarge     ="C5d9xlarge";
                public static string  C5dLarge       ="C5dLarge";
                public static string  C5dXlarge      ="C5dXlarge";
                public static string  C5Large        ="C5Large";
                public static string  C5Xlarge       ="C5Xlarge";
                public static string  Cc14xlarge     ="Cc14xlarge";
                public static string  Cc28xlarge     ="Cc28xlarge";
                public static string  Cg14xlarge     ="Cg14xlarge";
                public static string  Cr18xlarge     ="Cr18xlarge";
                public static string  D22xlarge      ="D22xlarge";
                public static string  D24xlarge      ="D24xlarge";
                public static string  D28xlarge      ="D28xlarge";
                public static string  D2Xlarge       ="D2Xlarge";
                public static string  F116xlarge     ="F116xlarge";
                public static string  F12xlarge      ="F12xlarge";
                public static string  G22xlarge      ="G22xlarge";
                public static string  G28xlarge      ="G28xlarge";
                public static string  G316xlarge     ="G316xlarge";
                public static string  G34xlarge      ="G34xlarge";
                public static string  G38xlarge      ="G38xlarge";
                public static string  H116xlarge     ="H116xlarge";
                public static string  H12xlarge      ="H12xlarge";
                public static string  H14xlarge      ="H14xlarge";
                public static string  H18xlarge      ="H18xlarge";
                public static string  Hi14xlarge     ="Hi14xlarge";
                public static string  Hs18xlarge     ="Hs18xlarge";
                public static string  I22xlarge      ="I22xlarge";
                public static string  I24xlarge      ="I24xlarge";
                public static string  I28xlarge      ="I28xlarge";
                public static string  I2Xlarge       ="I2Xlarge";
                public static string  I316xlarge     ="I316xlarge";
                public static string  I32xlarge      ="I32xlarge";
                public static string  I34xlarge      ="I34xlarge";
                public static string  I38xlarge      ="I38xlarge";
                public static string  I3Large        ="I3Large";
                public static string  I3Metal        ="I3Metal";
                public static string  I3Xlarge       ="I3Xlarge";
                public static string  M1Large        ="M1Large";
                public static string  M1Medium       ="M1Medium";
                public static string  M1Small        ="M1Small";
                public static string  M1Xlarge       ="M1Xlarge";
                public static string  M22xlarge      ="M22xlarge";
                public static string  M24xlarge      ="M24xlarge";
                public static string  M2Xlarge       ="M2Xlarge";
                public static string  M32xlarge      ="M32xlarge";
                public static string  M3Large        ="M3Large";
                public static string  M3Medium       ="M3Medium";
                public static string  M3Xlarge       ="M3Xlarge";
                public static string  M410xlarge     ="M410xlarge";
                public static string  M416xlarge     ="M416xlarge";
                public static string  M42xlarge      ="M42xlarge";
                public static string  M44xlarge      ="M44xlarge";
                public static string  M4Large        ="M4Large";
                public static string  M4Xlarge       ="M4Xlarge";
                public static string  M512xlarge     ="M512xlarge";
                public static string  M524xlarge     ="M524xlarge";
                public static string  M52xlarge      ="M52xlarge";
                public static string  M54xlarge      ="M54xlarge";
                public static string  M5d12xlarge    ="M5d12xlarge";
                public static string  M5d24xlarge    ="M5d24xlarge";
                public static string  M5d2xlarge     ="M5d2xlarge";
                public static string  M5d4xlarge     ="M5d4xlarge";
                public static string  M5dLarge       ="M5dLarge";
                public static string  M5dXlarge      ="M5dXlarge";
                public static string  M5Large        ="M5Large";
                public static string  M5Xlarge       ="M5Xlarge";
                public static string  P216xlarge     ="P216xlarge";
                public static string  P28xlarge      ="P28xlarge";
                public static string  P2Xlarge       ="P2Xlarge";
                public static string  P316xlarge     ="P316xlarge";
                public static string  P32xlarge      ="P32xlarge";
                public static string  P38xlarge      ="P38xlarge";
                public static string  R32xlarge      ="R32xlarge";
                public static string  R34xlarge      ="R34xlarge";
                public static string  R38xlarge      ="R38xlarge";
                public static string  R3Large        ="R3Large";
                public static string  R3Xlarge       ="R3Xlarge";
                public static string  R416xlarge     ="R416xlarge";
                public static string  R42xlarge      ="R42xlarge";
                public static string  R44xlarge      ="R44xlarge";
                public static string  R48xlarge      ="R48xlarge";
                public static string  R4Large        ="R4Large";
                public static string  R4Xlarge       ="R4Xlarge";
                public static string  R512xlarge     ="R512xlarge";
                public static string  R516xlarge     ="R516xlarge";
                public static string  R524xlarge     ="R524xlarge";
                public static string  R52xlarge      ="R52xlarge";
                public static string  R54xlarge      ="R54xlarge";
                public static string  R58xlarge      ="R58xlarge";
                public static string  R5d12xlarge    ="R5d12xlarge";
                public static string  R5d16xlarge    ="R5d16xlarge";
                public static string  R5d24xlarge    ="R5d24xlarge";
                public static string  R5d2xlarge     ="R5d2xlarge";
                public static string  R5d4xlarge     ="R5d4xlarge";
                public static string  R5d8xlarge     ="R5d8xlarge";
                public static string  R5dLarge       ="R5dLarge";
                public static string  R5dMetal       ="R5dMetal";
                public static string  R5dXlarge      ="R5dXlarge";
                public static string  R5Large        ="R5Large";
                public static string  R5Metal        ="R5Metal";
                public static string  R5Xlarge       ="R5Xlarge";
                public static string  T1Micro        ="T1Micro";
                public static string  T22xlarge      ="T22xlarge";
                public static string  T2Large        ="T2Large";
                public static string  T2Medium       ="T2Medium";
                public static string  T2Micro        ="T2Micro";
                public static string  T2Nano         ="T2Nano";
                public static string  T2Small        ="T2Small";
                public static string  T2Xlarge       ="T2Xlarge";
                public static string  T32xlarge      ="T32xlarge";
                public static string  T3Large        ="T3Large";
                public static string  T3Medium       ="T3Medium";
                public static string  T3Micro        ="T3Micro";
                public static string  T3Nano         ="T3Nano";
                public static string  T3Small        ="T3Small";
                public static string  T3Xlarge       ="T3Xlarge";
                public static string  X116xlarge     ="X116xlarge";
                public static string  X132xlarge     ="X132xlarge";
                public static string  X1e16xlarge    = "X1e16xlarge";
                public static string  X1e2xlarge     ="X1e2xlarge";
                public static string  X1e32xlarge    ="X1e32xlarge";
                public static string  X1e4xlarge     ="X1e4xlarge";
                public static string  X1e8xlarge     ="X1e8xlarge";
                public static string  X1eXlarge      ="X1eXlarge";
                public static string  Z1d12xlarge    ="Z1d12xlarge";
                public static string  Z1d2xlarge     ="Z1d2xlarge";
                public static string  Z1d3xlarge     ="Z1d3xlarge";
                public static string  Z1d6xlarge     ="Z1d6xlarge";
                public static string  Z1dLarge       ="Z1dLarge";
                public static string  Z1dXlarge       ="Z1dXlarge";
            public static String[] AllInstanceTypes = new String[] {
                C1Medium
                ,C1Xlarge
                ,C32xlarge
                ,C34xlarge
                ,C38xlarge
                ,C3Large
                ,C3Xlarge
                ,C42xlarge
                ,C44xlarge
                ,C48xlarge
                ,C4Large
                ,C4Xlarge
                ,C518xlarge
                ,C52xlarge
                ,C54xlarge
                ,C59xlarge
                ,C5d18xlarge
                ,C5d2xlarge
                ,C5d4xlarge
                ,C5d9xlarge
                ,C5dLarge
                ,C5dXlarge
                ,C5Large
                ,C5Xlarge
                ,Cc14xlarge
                ,Cc28xlarge
                ,Cg14xlarge
                ,Cr18xlarge
                ,D22xlarge
                ,D24xlarge
                ,D28xlarge
                ,D2Xlarge
                ,F116xlarge
                ,F12xlarge
                ,G22xlarge
                ,G28xlarge
                ,G316xlarge
                ,G34xlarge
                ,G38xlarge
                ,H116xlarge
                ,H12xlarge
                ,H14xlarge
                ,H18xlarge
                ,Hi14xlarge
                ,Hs18xlarge
                ,I22xlarge
                ,I24xlarge
                ,I28xlarge
                ,I2Xlarge
                ,I316xlarge
                ,I32xlarge
                ,I34xlarge
                ,I38xlarge
                ,I3Large
                ,I3Metal
                ,I3Xlarge
                ,M1Large
                ,M1Medium
                ,M1Small
                ,M1Xlarge
                ,M22xlarge
                ,M24xlarge
                ,M2Xlarge
                ,M32xlarge
                ,M3Large
                ,M3Medium
                ,M3Xlarge
                ,M410xlarge
                ,M416xlarge
                ,M42xlarge
                ,M44xlarge
                ,M4Large
                ,M4Xlarge
                ,M512xlarge
                ,M524xlarge
                ,M52xlarge
                ,M54xlarge
                ,M5d12xlarge
                ,M5d24xlarge
                ,M5d2xlarge
                ,M5d4xlarge
                ,M5dLarge
                ,M5dXlarge
                ,M5Large
                ,M5Xlarge
                ,P216xlarge
                ,P28xlarge
                ,P2Xlarge
                ,P316xlarge
                ,P32xlarge
                ,P38xlarge
                ,R32xlarge
                ,R34xlarge
                ,R38xlarge
                ,R3Large
                ,R3Xlarge
                ,R416xlarge
                ,R42xlarge
                ,R44xlarge
                ,R48xlarge
                ,R4Large
                ,R4Xlarge
                ,R512xlarge
                ,R516xlarge
                ,R524xlarge
                ,R52xlarge
                ,R54xlarge
                ,R58xlarge
                ,R5d12xlarge
                ,R5d16xlarge
                ,R5d24xlarge
                ,R5d2xlarge
                ,R5d4xlarge
                ,R5d8xlarge
                ,R5dLarge
                ,R5dMetal
                ,R5dXlarge
                ,R5Large
                ,R5Metal
                ,R5Xlarge
                ,T1Micro
                ,T22xlarge
                ,T2Large
                ,T2Medium
                ,T2Micro
                ,T2Nano
                ,T2Small
                ,T2Xlarge
                ,T32xlarge
                ,T3Large
                ,T3Medium
                ,T3Micro
                ,T3Nano
                ,T3Small
                ,T3Xlarge
                ,X116xlarge
                ,X132xlarge
                ,X1e16xlarge
                ,X1e2xlarge
                ,X1e32xlarge
                ,X1e4xlarge
                ,X1e8xlarge
                ,X1eXlarge
                ,Z1d12xlarge
                ,Z1d2xlarge
                ,Z1d3xlarge
                ,Z1d6xlarge
                ,Z1dLarge
                ,Z1dXlarge
            };
            public static InstanceType[] AllActualInstanceTypes = new InstanceType[] {
                 InstanceType.C1Medium     
                 ,InstanceType.C1Xlarge 
                 ,InstanceType.C32xlarge
                 ,InstanceType.C34xlarge
                 ,InstanceType.C38xlarge
                 ,InstanceType.C3Large 
                 ,InstanceType.C3Xlarge 
                 ,InstanceType.C42xlarge
                 ,InstanceType.C44xlarge
                 ,InstanceType.C48xlarge
                 ,InstanceType.C4Large 
                 ,InstanceType.C4Xlarge 
                 ,InstanceType.C518xlarge
                 ,InstanceType.C52xlarge
                 ,InstanceType.C54xlarge
                 ,InstanceType.C59xlarge
                 ,InstanceType.C5d18xlarge
                 ,InstanceType.C5d2xlarge
                 ,InstanceType.C5d4xlarge
                 ,InstanceType.C5d9xlarge
                 ,InstanceType.C5dLarge 
                 ,InstanceType.C5dXlarge
                 ,InstanceType.C5Large 
                 ,InstanceType.C5Xlarge 
                 ,InstanceType.Cc14xlarge
                 ,InstanceType.Cc28xlarge
                 ,InstanceType.Cg14xlarge
                 ,InstanceType.Cr18xlarge
                 ,InstanceType.D22xlarge
                 ,InstanceType.D24xlarge
                 ,InstanceType.D28xlarge
                 ,InstanceType.D2Xlarge 
                 ,InstanceType.F116xlarge
                 ,InstanceType.F12xlarge
                 ,InstanceType.G22xlarge
                 ,InstanceType.G28xlarge
                 ,InstanceType.G316xlarge 
                 ,InstanceType.G34xlarge
                 ,InstanceType.G38xlarge
                 ,InstanceType.H116xlarge
                 ,InstanceType.H12xlarge
                 ,InstanceType.H14xlarge
                 ,InstanceType.H18xlarge
                 ,InstanceType.Hi14xlarge
                 ,InstanceType.Hs18xlarge
                 ,InstanceType.I22xlarge
                 ,InstanceType.I24xlarge
                 ,InstanceType.I28xlarge
                 ,InstanceType.I2Xlarge 
                 ,InstanceType.I316xlarge
                 ,InstanceType.I32xlarge
                 ,InstanceType.I34xlarge
                 ,InstanceType.I38xlarge
                 ,InstanceType.I3Large 
                 ,InstanceType.I3Metal 
                 ,InstanceType.I3Xlarge 
                 ,InstanceType.M1Large 
                 ,InstanceType.M1Medium 
                 ,InstanceType.M1Small 
                 ,InstanceType.M1Xlarge 
                 ,InstanceType.M22xlarge
                 ,InstanceType.M24xlarge
                 ,InstanceType.M2Xlarge 
                 ,InstanceType.M32xlarge
                 ,InstanceType.M3Large 
                 ,InstanceType.M3Medium 
                 ,InstanceType.M3Xlarge 
                 ,InstanceType.M410xlarge
                 ,InstanceType.M416xlarge
                 ,InstanceType.M42xlarge
                 ,InstanceType.M44xlarge
                 ,InstanceType.M4Large 
                 ,InstanceType.M4Xlarge 
                 ,InstanceType.M512xlarge
                 ,InstanceType.M524xlarge
                 ,InstanceType.M52xlarge
                 ,InstanceType.M54xlarge
                 ,InstanceType.M5d12xlarge
                 ,InstanceType.M5d24xlarge
                 ,InstanceType.M5d2xlarge
                 ,InstanceType.M5d4xlarge
                 ,InstanceType.M5dLarge 
                 ,InstanceType.M5dXlarge
                 ,InstanceType.M5Large 
                 ,InstanceType.M5Xlarge 
                 ,InstanceType.P216xlarge
                 ,InstanceType.P28xlarge
                 ,InstanceType.P2Xlarge 
                 ,InstanceType.P316xlarge
                 ,InstanceType.P32xlarge
                 ,InstanceType.P38xlarge
                 ,InstanceType.R32xlarge
                 ,InstanceType.R34xlarge
                 ,InstanceType.R38xlarge
                 ,InstanceType.R3Large 
                 ,InstanceType.R3Xlarge 
                 ,InstanceType.R416xlarge
                 ,InstanceType.R42xlarge
                 ,InstanceType.R44xlarge
                 ,InstanceType.R48xlarge
                 ,InstanceType.R4Large 
                 ,InstanceType.R4Xlarge 
                 ,InstanceType.R512xlarge
                 ,InstanceType.R516xlarge
                 ,InstanceType.R524xlarge
                 ,InstanceType.R52xlarge
                 ,InstanceType.R54xlarge
                 ,InstanceType.R58xlarge
                 ,InstanceType.R5d12xlarge
                 ,InstanceType.R5d16xlarge
                 ,InstanceType.R5d24xlarge
                 ,InstanceType.R5d2xlarge
                 ,InstanceType.R5d4xlarge
                 ,InstanceType.R5d8xlarge
                 ,InstanceType.R5dLarge 
                 ,InstanceType.R5dMetal 
                 ,InstanceType.R5dXlarge
                 ,InstanceType.R5Large 
                 ,InstanceType.R5Metal 
                 ,InstanceType.R5Xlarge 
                 ,InstanceType.T1Micro 
                 ,InstanceType.T22xlarge
                 ,InstanceType.T2Large 
                 ,InstanceType.T2Medium 
                 ,InstanceType.T2Micro 
                 ,InstanceType.T2Nano 
                 ,InstanceType.T2Small 
                 ,InstanceType.T2Xlarge 
                 ,InstanceType.T32xlarge
                 ,InstanceType.T3Large 
                 ,InstanceType.T3Medium 
                 ,InstanceType.T3Micro 
                 ,InstanceType.T3Nano
                 ,InstanceType.T3Small 
                 ,InstanceType.T3Xlarge 
                 ,InstanceType.X116xlarge
                 ,InstanceType.X132xlarge
                 ,InstanceType.X1e16xlarge
                 ,InstanceType.X1e2xlarge
                 ,InstanceType.X1e32xlarge
                 ,InstanceType.X1e4xlarge
                 ,InstanceType.X1e8xlarge
                 ,InstanceType.X1eXlarge
                 ,InstanceType.Z1d12xlarge
                 ,InstanceType.Z1d2xlarge
                 ,InstanceType.Z1d3xlarge
                 ,InstanceType.Z1d6xlarge
                 ,InstanceType.Z1dLarge
                 ,InstanceType.Z1dXlarge
            };
            public static InstanceType InstanceTypeFromName(String name)
            {
                Console.WriteLine(name);
                Console.WriteLine("AllActualInstanceTypes length " + AllActualInstanceTypes.Length);
                Console.WriteLine("AllInstanceTypes length " + AllInstanceTypes.Length);
                return AllActualInstanceTypes[AllInstanceTypes.ToList().IndexOf(name)];
            }

        }
        public static class Regions
        {
            public static string USEast1 = "USEast1";
            public static string USEast2 = "USEast2";
            public static string USWest1 = "USWest1";
            public static string USWest2 = "USWest2";
            public static string EUWest1 = "EUWest1";
            public static string EUWest2 = "EUWest2";
            public static string EUWest3 = "EUWest3";
            public static string EUCentral1 = "EUCentral1";
            public static string APNortheast1 = "APNortheast1";
            public static string APNortheast2 = "APNortheast2";
            public static string APSouth1 = "APSouth1";
            public static string APSoutheast1 = "APSoutheast1";
            public static string APSoutheast2 = "APSoutheast2";
            public static string SAEast1 = "SAEast1";
            public static string USGovCloudWest1 = "USGovCloudWest1";
            public static string CNNorth1 = "CNNorth1";
            public static string CNNorthWest1 = "CNNorthWest1";
            public static string CACentral1 = "CACentral1";
            public static String[] AllRegions = new String[] {
             USEast1         ,
             USEast2         ,
             USWest1         ,
             USWest2         ,
             EUWest1         ,
             EUWest2         ,
             EUWest3         ,
             EUCentral1      ,
             APNortheast1    ,
             APNortheast2    ,
             APSouth1        ,
             APSoutheast1    ,
             APSoutheast2    ,
             SAEast1         ,
             USGovCloudWest1 ,
             CNNorth1        ,
             CNNorthWest1    ,
             CACentral1      ,
        };
            public static RegionEndpoint[] AllActualRegions = new RegionEndpoint[] {
             RegionEndpoint.USEast1         ,
             RegionEndpoint.USEast2         ,
             RegionEndpoint.USWest1         ,
             RegionEndpoint.USWest2         ,
             RegionEndpoint.EUWest1         ,
             RegionEndpoint.EUWest2         ,
             RegionEndpoint.EUWest3         ,
             RegionEndpoint.EUCentral1      ,
             RegionEndpoint.APNortheast1    ,
             RegionEndpoint.APNortheast2    ,
             RegionEndpoint.APSouth1        ,
             RegionEndpoint.APSoutheast1    ,
             RegionEndpoint.APSoutheast2    ,
             RegionEndpoint.SAEast1         ,
             RegionEndpoint.USGovCloudWest1 ,
             RegionEndpoint.CNNorth1        ,
             RegionEndpoint.CNNorthWest1    ,
             RegionEndpoint.CACentral1      ,
        };
            public static RegionEndpoint RegionEndpointFromName(String name)
            {
                return AllActualRegions[AllRegions.ToList().IndexOf(name)];
            }
        }
    }
}
