using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using CloudStorageActivity.Utils;
using static CloudStorageActivity.Aws.Ec2.EC2Connector;

namespace CloudStorageActivity.Aws.DynamoDB
{
    public class DynamoDBConnector
    {
        private AmazonDynamoDBClient client;

        public DynamoDBConnector(String myaccesskey, String mysecretkey, String region)
        {
            var awsCredentials = new BasicAWSCredentials(myaccesskey, mysecretkey);
            client = new AmazonDynamoDBClient(awsCredentials, Regions.RegionEndpointFromName(region));
        }
        public String[] Query(String tableName,String expression=null,String[] attributes =null)
        {
            var results = new List<string>();
            Table table = Table.LoadTable(client, tableName);
            

            QueryOperationConfig config = new QueryOperationConfig()
            {
                Select = SelectValues.AllAttributes,
                ConsistentRead = true
            };
            if (attributes != null)
            {  config. Select = SelectValues.SpecificAttributes;
                config.AttributesToGet = new List<string> (attributes);
            }
            if (expression != null)
            {
                config.Filter = CreateFilter(expression);
            }

            Search tableResult = table.Query(config);
            var docs = tableResult.GetRemaining();
            foreach (Document doc in docs)
                results.Add(doc.ToJson());
           return results.ToArray();
        }
        public void PutItem(String tableName,String jsonItem)
        {
            var item = Document.FromJson(jsonItem);
            Table table = Table.LoadTable(client, tableName);
            table.PutItem(item);
        }
        public void DeleteItem(String tableName, String jsonItem)
        {
            var item = Document.FromJson(jsonItem);
            Table table = Table.LoadTable(client, tableName);
            table.DeleteItem(item);
        }

        public void UpdateItem(String tableName, String jsonItem)
        {
            var item = Document.FromJson(jsonItem);
            Table table = Table.LoadTable(client, tableName);
            table.DeleteItem(item);
        }
        private QueryFilter CreateFilter(String expression) {
            String[] expressionItems = StringUtils.SplitExpressionToArray(expression);
            QueryFilter filter = new QueryFilter();

            for(int i=0;i< expressionItems.Length;i++)
            {
                String expressionItem = expressionItems[i];
                if (ConditionExpression.AllConditions.Contains(expressionItem))
                {

                    filter.AddCondition(expressionItems[i-1], ConditionExpression.QueryOperatorFromName(expressionItem), expressionItems[i + 1]);
                }
            }
            return filter;
        }
        private class ConditionExpression
        {
            public static String Equal = "=";
            public static String LessThanOrEqual = "<=";
            public static String LessThan = "<";
            public static String GreaterThanOrEqual = ">=";
            public static String GreaterThan = ">";
            public static String[] AllConditions = new String[] {
             Equal, LessThanOrEqual,LessThan , GreaterThanOrEqual ,GreaterThan
        };
            public static QueryOperator[] AllQueryOperatorConditions = new QueryOperator[] {
             QueryOperator.Equal, QueryOperator.LessThanOrEqual,QueryOperator.LessThan , QueryOperator.GreaterThanOrEqual ,QueryOperator.GreaterThan
        };
            public static QueryOperator QueryOperatorFromName(String name)
            {
                return AllQueryOperatorConditions[AllConditions.ToList().IndexOf(name)];
            }
        }
    }
}
