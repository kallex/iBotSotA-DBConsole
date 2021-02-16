using System.Runtime.CompilerServices;
using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;

namespace InfraSetup
{
    public static class DynamoDBStack
    {
        public static void Setup(Stack stack)
        {
            var tableName = "AccountSummary";
            var id = tableName;
            var table = new Table(stack, id, new TableProps()
            {
                TableName = tableName,
                PartitionKey = new Attribute
                {
                    Type = AttributeType.STRING,
                    Name = "AccountID"
                },
                BillingMode = BillingMode.PROVISIONED
                //BillingMode = BillingMode.PAY_PER_REQUEST
            });
        }
    }
}