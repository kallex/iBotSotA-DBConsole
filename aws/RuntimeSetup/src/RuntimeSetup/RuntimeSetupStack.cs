using System;
using System.Diagnostics;
using System.Linq;
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
        private string[] ValidBuildNumberPrefixes = new[] {"dev", "test", "beta"};

        internal RuntimeSetupStack(Construct scope, string id, EnvironmentDetails envDetails, IStackProps props = null) : base(scope, id, props)
        {
            StackDependency dependencyInfo = new StackDependency();
            //var value = buildNumberParameter.Resolve(scope.Node.)
            var buildNumber = System.Environment.GetEnvironmentVariable("BUILD_NUMBER");

            var lambdaFunction = LambdaStack.Setup(this, dependencyInfo, envDetails, buildNumber);
            var api = APIGatewayStack.Setup(this, dependencyInfo, envDetails, lambdaFunction);

            //var fargate = FargateStack.Setup(this, dependencyInfo, envDetails, buildNumber);
            var fargate = FargateStack.SetupFG(this, dependencyInfo, envDetails, buildNumber);

            // The code that defines your stack goes here
        }
    }
}
