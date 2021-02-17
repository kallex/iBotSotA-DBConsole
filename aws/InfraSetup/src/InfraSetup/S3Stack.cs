using Amazon.CDK;
using Amazon.CDK.AWS.S3;

namespace InfraSetup
{
    public static class S3Stack
    {
        public static void Setup(Stack stack, EnvironmentDetails envDetails)
        {
            var idName = $"{envDetails.AppPrefix}-s3-{envDetails.EnvSuffix}";
            var bucket = new Bucket(stack, idName, new BucketProps()
            {
                BucketName = idName,
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
            });
        }
    }
}