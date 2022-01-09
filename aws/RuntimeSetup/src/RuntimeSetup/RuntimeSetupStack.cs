using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.Route53;

namespace RuntimeSetup
{
    public enum EnvironmentType
    {
        Undefined = 0,
        Shared,
        Dev,
        Test,
        Beta,
        Prod,
    }
    public class EnvironmentDetails
    {
        public EnvironmentType Type;
        public string AppPrefix;
        public string EnvSuffix;
    }

    public class RuntimeSetupStack
    {
        private string[] ValidBuildNumberPrefixes = new[] {"dev", "test", "beta"};

        public static void Setup(Construct app, SharedConstructs sharedConstruct, string id, EnvironmentDetails envDetails, StackProps props)
        {
            //var parentStack = sharedConstruct.Stack;
            //var stack = new Stack(parentStack, id, props);
            var stack = new Stack(app, id, props);
            stack.AddDependency(sharedConstruct.Stack);

            //var value = buildNumberParameter.Resolve(scope.Node.)
            var buildNumber = System.Environment.GetEnvironmentVariable("BUILD_NUMBER");
            var lambdaFunction = LambdaStack.Setup(stack, envDetails, buildNumber);
            var api = APIGatewayStack.Setup(stack, envDetails, lambdaFunction);

            var fargate = FargateStack.Setup(stack, sharedConstruct, envDetails, buildNumber);

            // The code that defines your stack goes here

        }

        public static SharedConstructs SetupShared(Construct app, string id, EnvironmentDetails envDetails,
            string[] sharedEnvironments, StackProps props)
        {
            var vpcId = $"fgvpc";
            var clusterId = $"fgcluster";

            var stack = new Stack(app, id, props);
            /*
            return new SharedConstructs()
            {
                Stack = stack,
            };
            */
            //return null;

            var vpc = new Vpc(stack, vpcId, new VpcProps
            {
                MaxAzs = 2, // Default is all AZs in region
                NatGateways = 0
            });

            var cluster = new Cluster(stack, clusterId, new ClusterProps
            {
                Vpc = vpc,
                EnableFargateCapacityProviders = true,
                ContainerInsights = true,
            });

            var zoneName = "ibotsota.net";
            string hostedZoneID = Constant.HostedZoneID;
            var hostedZone = HostedZone.FromHostedZoneAttributes(stack, $"hostedzone-{zoneName}-fargate",
                new HostedZoneAttributes()
                {
                    HostedZoneId = hostedZoneID,
                    ZoneName = zoneName
                });

            string albId = "iBotSotA-Shr-LB";
            var loadBalancer = new ApplicationLoadBalancer(stack, albId, new ApplicationLoadBalancerProps()
            {
                InternetFacing = true,
                Vpc = vpc,
            });


            Dictionary<string, ApplicationTargetGroup> targetGroupDict =
                new Dictionary<string, ApplicationTargetGroup>();

            List<IListenerCertificate> listenerCerts = new List<IListenerCertificate>();

            foreach (var envName in sharedEnvironments)
            {
                var domainName = $"fg-{envName}.{zoneName}";

                var dnsCert = new DnsValidatedCertificate(stack, $"cert-ibotsota-{envName}-fargate",
                    new DnsValidatedCertificateProps()
                    {
                        DomainName = domainName,
                        HostedZone = hostedZone
                    });
                var listenerCert = new ListenerCertificate(dnsCert.CertificateArn);
                listenerCerts.Add(listenerCert);

                var cNameID = $"cname-{domainName}";
                var route53 = new CnameRecord(stack, cNameID, new CnameRecordProps()
                {
                    RecordName = domainName,
                    Ttl = Duration.Minutes(5),
                    DomainName = loadBalancer.LoadBalancerDnsName,
                    Zone = hostedZone,
                    Comment = "CloudFormation Stack maintained"
                });
            }

            var listenerId = $"shr-listener";
            var listener = loadBalancer.AddListener(listenerId, new BaseApplicationListenerProps()
            {
                Protocol = ApplicationProtocol.HTTPS,
                Certificates = listenerCerts.ToArray(),
                Open = true,
            });
            var targetGroupId = $"shr-target";
            var targetGroup = listener.AddTargets(targetGroupId, new AddApplicationTargetsProps()
            {
                Protocol = ApplicationProtocol.HTTP,
                Port = 80,
            });
            //targetGroupDict.Add(envName, targetGroup);

            var sharedConstructs = new SharedConstructs
            {
                Stack = stack,
                Cluster = cluster,
                Vpc = vpc,
                HostedZone = hostedZone,
                LoadBalancer = loadBalancer,
                TargetGroup = targetGroup
            };


            return sharedConstructs;
        }
    }
}
