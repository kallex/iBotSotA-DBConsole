using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.S3;

namespace RuntimeSetup
{
    public class LambdaStack
    {
        public static Function Setup(Stack stack, StackDependency stackDependency, EnvironmentDetails envDetails,
            string buildNumber)
        {
            var idName = $"{envDetails.AppPrefix}-AspNetCore-{envDetails.EnvSuffix}";

            var envName = envDetails.EnvSuffix;

            /*
            if (buildNumber?.StartsWith(envName) == false)
            {
                throw new ArgumentException($"Build number '{buildNumber}' does not match with environment '{envName}'");
            }
            */

            buildNumber ??= "BUILD_NUMBER";

            var lambdaPackageKey = $"{envName}/iBotSotALambda_{buildNumber}.zip";

            string devopsbucketIdName = "ibotsota-devops";
            var bucket = Bucket.FromBucketName(stack, devopsbucketIdName, devopsbucketIdName);
            var function = new Function(stack, idName, new FunctionProps()
            {
                Environment = new Dictionary<string, string>()
                {
                    { "Environment", envDetails.EnvSuffix }
                },
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromBucket(bucket, lambdaPackageKey),
                Handler = "iBotSotALambda::iBotSotALambda.LambdaEntryPoint::FunctionHandlerAsync",
                Tracing = Tracing.ACTIVE,
                Timeout = Duration.Minutes(1)
            });
            var managedPolcyID = $"{idName}-Policy";
            function.Role.AddManagedPolicy(ManagedPolicy.FromManagedPolicyArn(stack, managedPolcyID, "arn:aws:iam::394301006475:policy/iBotSotA-OperatorPolicy"));

            return function;
        }
        
    }
}