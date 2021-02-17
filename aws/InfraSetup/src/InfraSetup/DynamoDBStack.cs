using System.Runtime.CompilerServices;
using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;

namespace InfraSetup
{
    public static class DynamoDBStack
    {
        public static void Setup(Stack stack, EnvironmentDetails envDetails)
        {
            var tableItems = new[] {(tableName: "Account", partitionKeyName:"AccountID")};
            foreach (var tableItem in tableItems)
            {
                var tableName = tableItem.tableName;
                var partitionKeyName = tableItem.partitionKeyName;

                var idName = $"{envDetails.AppPrefix}-{tableName}-{envDetails.EnvSuffix}";
                var table = new Table(stack, idName, new TableProps()
                {
                    TableName = idName,
                    PartitionKey = new Attribute
                    {
                        Type = AttributeType.STRING,
                        Name = partitionKeyName
                    },
                    BillingMode = BillingMode.PROVISIONED
                });
            }
        }
    }
}