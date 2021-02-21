using Amazon.CDK;

namespace InfraSetup
{
    public enum EnvironmentType
    {
        Undefined = 0,
        Dev,
        Test,
        Beta,
        Prod,
    }
    public class EnvironmentDetails
    {
        public EnvironmentType Type;
        public string AppPrefix;
        public string EnvSuffix;
    }

    public class InfraSetupStack : Stack
    {
        internal InfraSetupStack(Construct scope, string id, EnvironmentDetails envDetails, IStackProps props = null) : base(scope, id, props)
        {
            ResourceGroupStack.Setup(this, envDetails);
            S3Stack.Setup(this, envDetails);
            TimestreamStack.Setup(this, envDetails);
            DynamoDBStack.Setup(this, envDetails);
        }
    }
}
