using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Environment = Amazon.CDK.Environment;

namespace RuntimeSetup
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            Environment env = getEnvironment();
            StackProps stackProps = new StackProps
            {
                Env = env,
            };
            var appName = "ibotsota";
            var sharedEnvironments = new string[] { "dev", "test", "beta" };
            var sharedId = "iBotSotA-Shr-Runtime";
            
            SharedConstructs sharedConstruct = RuntimeSetupStack.SetupShared(app, sharedId,
                new EnvironmentDetails() { AppPrefix = appName, EnvSuffix = "shr", Type = EnvironmentType.Shared }, sharedEnvironments, stackProps);

            //var sharedStack = sharedConstruct.Stack;



            RuntimeSetupStack.Setup(app, sharedConstruct,"iBotSotA-dev-Runtime", new EnvironmentDetails() { AppPrefix = appName, EnvSuffix = "dev", Type = EnvironmentType.Dev }, stackProps);
            RuntimeSetupStack.Setup(app, sharedConstruct, "iBotSotA-test-Runtime", new EnvironmentDetails() { AppPrefix = appName, EnvSuffix = "test", Type = EnvironmentType.Test }, stackProps);
            RuntimeSetupStack.Setup(app, sharedConstruct, "iBotSotA-beta-Runtime", new EnvironmentDetails() { AppPrefix = appName, EnvSuffix = "beta", Type = EnvironmentType.Beta }, stackProps);
            app.Synth();
        }
        private static Environment getEnvironment()
        {
            return new Environment()
            {
                Region = "eu-west-1",
            };
        }

    }
}
