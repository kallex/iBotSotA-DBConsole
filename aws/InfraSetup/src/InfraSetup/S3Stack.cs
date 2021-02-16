using Amazon.CDK;
using Amazon.CDK.AWS.S3;

namespace InfraSetup
{
    public static class S3Stack
    {
        public static void Setup(Stack stack)
        {
            var id = "ibotsota-bucket";
            var bucket = new Bucket(stack, id, new BucketProps()
            {
                BucketName = "ibotsota-s3",
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
            });
        }
    }
}