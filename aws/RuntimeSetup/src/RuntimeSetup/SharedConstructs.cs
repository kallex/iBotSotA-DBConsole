using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.Route53;

namespace RuntimeSetup
{
    public class SharedConstructs
    {
        public IHostedZone HostedZone { get; set; }
        public Stack Stack { get; set; }
        public ICluster Cluster { get; set; }
        public IVpc Vpc { get; set; }
        public ApplicationLoadBalancer LoadBalancer { get; set; }
        public Dictionary<string, ApplicationTargetGroup> TargetGroupDict { get; set; }
    }
}