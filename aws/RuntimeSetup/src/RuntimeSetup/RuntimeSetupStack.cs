using Amazon.CDK;

namespace RuntimeSetup
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

    public class RuntimeSetupStack : Stack
    {
        internal RuntimeSetupStack(Construct scope, string id, EnvironmentDetails envDetails, IStackProps props = null) : base(scope, id, props)
        {
            StackDependency dependencyInfo = new StackDependency();

            APIGatewayStack.Setup(this, envDetails);
            LambdaStack.Setup(this, dependencyInfo, envDetails);

            // The code that defines your stack goes here
        }
    }
}
