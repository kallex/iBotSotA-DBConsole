using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;
using Environment = Amazon.CDK.Environment;

namespace InfraSetup
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            Environment env = getEnvironment();
            StackProps stackProps = new StackProps
            {
                Env = env
            };
            var appName = "ibotsota";
            var devStack = new InfraSetupStack(app, "DevStack", new EnvironmentDetails() { AppPrefix = appName, EnvSuffix = "dev", Type = EnvironmentType.Dev }, stackProps);
            var betaStack = new InfraSetupStack(app, "BetaStack", new EnvironmentDetails() { AppPrefix = appName, EnvSuffix = "beta", Type = EnvironmentType.Beta }, stackProps);
            //var infraStack = new InfraSetupStack(app, "InfraSetupStack", stackProps);
            app.Synth();
        }
        private static Environment getEnvironment()
        {
            return new Environment()
            {
                Region = "eu-west-1"
            };
        }
    }
}
