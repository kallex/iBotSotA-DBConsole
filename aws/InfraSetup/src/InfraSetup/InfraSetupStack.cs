using Amazon.CDK;

namespace InfraSetup
{
    public class InfraSetupStack : Stack
    {
        internal InfraSetupStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            ResourceGroupStack.Setup(this);
            S3Stack.Setup(this);
            TimestreamStack.Setup(this);
            DynamoDBStack.Setup(this);
        }
    }
}
