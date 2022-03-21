using Amazon;
using Amazon.S3;

namespace AWSDataServices
{
    public class S3DataService
    {
        public AmazonS3Client S3Client { get; }

        public S3DataService(RegionEndpoint region)
        {
            S3Client = new AmazonS3Client(region);
        }
    }
}