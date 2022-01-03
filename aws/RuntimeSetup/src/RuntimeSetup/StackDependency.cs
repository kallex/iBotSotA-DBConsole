using Amazon.CDK.AWS.Route53;

namespace RuntimeSetup
{
    public class StackDependency
    {
        public IHostedZone HostedZone { get; set; }
    }
}