using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace CloudStorageActivity.Aws.Sns
{
    public class SnsConnector
    {
        private AmazonSimpleNotificationServiceClient client;
        public SnsConnector(String myaccesskey, String mysecretkey, String region)
        {
            var awsCredentials = new BasicAWSCredentials(myaccesskey, mysecretkey);
            client = new AmazonSimpleNotificationServiceClient(awsCredentials, Regions.RegionEndpointFromName(region));
        }
        public void SendSms(String message,String phoneNumber,String senderID,String SMSType= "Transactional")
        {
            MessageAttributeValue mMessageAttributeValue = new MessageAttributeValue();
            mMessageAttributeValue.DataType    = "String";
            mMessageAttributeValue.StringValue = senderID;

            MessageAttributeValue mMessageAttributeValue2 = new MessageAttributeValue();
            mMessageAttributeValue2.DataType = "String";
            mMessageAttributeValue2.StringValue = SMSType;

            Dictionary<String, MessageAttributeValue> smsAttributes =
                    new Dictionary<String, MessageAttributeValue>();
            smsAttributes.Add("DefaultSenderID", mMessageAttributeValue);
            smsAttributes.Add("DefaultSMSType", mMessageAttributeValue2);

            var mPublishRequest = new PublishRequest();
            mPublishRequest.Message = message;
            mPublishRequest.PhoneNumber = phoneNumber;
            mPublishRequest.MessageAttributes = smsAttributes;

           client.Publish(mPublishRequest);
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
