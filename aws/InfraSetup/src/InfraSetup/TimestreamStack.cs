using Amazon.CDK;
using Amazon.CDK.AWS.Timestream;

namespace InfraSetup
{
    public static class TimestreamStack
    {
        public static void Setup(Stack stack, EnvironmentDetails envDetails)
        {
            var idName = $"{envDetails.AppPrefix}-stats-{envDetails.EnvSuffix}";
            var databaseName = idName;
            new CfnDatabase(stack, idName, new CfnDatabaseProps()
            {
                DatabaseName = databaseName,
            });
            new CfnTable(stack, "Activity", new CfnTableProps()
            {
                TableName = "Activity",
                DatabaseName = databaseName,
            });
            new CfnTable(stack, "Session", new CfnTableProps()
            {
                TableName = "Session",
                DatabaseName = databaseName,
            });
        }
    }
}