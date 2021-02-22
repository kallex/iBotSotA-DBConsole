using Amazon.CDK;
using Amazon.CDK.AWS.S3;

namespace InfraSetup
{
    public static class S3Stack
    {
        public static void Setup(Stack stack, EnvironmentDetails envDetails)
        {
            var idName = $"{envDetails.AppPrefix}-{envDetails.EnvSuffix}";
            var bucket = new Bucket(stack, idName, new BucketProps()
            {
                BucketName = idName,
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
            });
            /*
            if (envDetails.Type == EnvironmentType.Dev)
            {
                var expName = $"exp-{idName}";
                var output = new CfnOutput(stack, "customname2", new CfnOutputProps()
                {
                    //ExportName = expName,
                    Value = stack.ExportValue(bucket, new ExportValueOptions()
                    {
                        Name = expName
                    })
                });
            }
            */
        }
    }
}