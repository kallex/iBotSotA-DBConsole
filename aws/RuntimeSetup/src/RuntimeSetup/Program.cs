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
                Env = env
            };
            var appName = "ibotsota";
            new RuntimeSetupStack(app, "iBotSotA-dev-Runtime", new EnvironmentDetails() { AppPrefix = appName, EnvSuffix = "dev", Type = EnvironmentType.Dev }, stackProps);
            new RuntimeSetupStack(app, "iBotSotA-test-Runtime", new EnvironmentDetails() { AppPrefix = appName, EnvSuffix = "test", Type = EnvironmentType.Test }, stackProps);
            new RuntimeSetupStack(app, "iBotSotA-beta-Runtime", new EnvironmentDetails() { AppPrefix = appName, EnvSuffix = "beta", Type = EnvironmentType.Beta }, stackProps);
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
