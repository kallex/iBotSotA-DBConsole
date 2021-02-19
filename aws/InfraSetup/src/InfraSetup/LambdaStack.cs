using System.Collections.Concurrent;
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.S3;

namespace InfraSetup
{
    public class LambdaStack
    {
        public static void Setup(Stack stack, StackDependency stackDependency, EnvironmentDetails envDetails)
        {
            /*
            var idName = $"{envDetails.AppPrefix}-AspNetCore-{envDetails.EnvSuffix}";
            var lambdaBucketIdName = $"{envDetails.AppPrefix}-lambda-{envDetails}";

            var bucket = new Bucket(stack, lambdaBucketIdName, new BucketProps()
            {
                BucketName = lambdaBucketIdName,
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
                PublicReadAccess = false,
                RemovalPolicy = RemovalPolicy.DESTROY,
            });

            var function = new Function(stack, idName, new FunctionProps()
            {
                Environment = new Dictionary<string, string>()
                {
                    { "Environment", envDetails.EnvSuffix }
                },
                AllowAllOutbound = true,
                Code = S3Code.FromBucket()
                
            });
            */
        }
        
    }
}