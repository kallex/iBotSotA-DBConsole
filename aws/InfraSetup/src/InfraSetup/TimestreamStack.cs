using Amazon.CDK;
using Amazon.CDK.AWS.Timestream;

namespace InfraSetup
{
    public static class TimestreamStack
    {
        public static void Setup(Stack stack)
        {
            var id = "ibotsota";
            var databaseName = id;
            new CfnDatabase(stack, id, new CfnDatabaseProps()
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