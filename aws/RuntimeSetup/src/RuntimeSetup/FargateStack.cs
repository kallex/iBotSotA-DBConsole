using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.AutoScaling;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.S3;
using ApplicationLoadBalancerProps = Amazon.CDK.AWS.ElasticLoadBalancingV2.ApplicationLoadBalancerProps;
using Protocol = Amazon.CDK.AWS.ECS.Protocol;

namespace RuntimeSetup
{
    public class FargateStack
    {

        public static ApplicationLoadBalancedFargateService SetupALBConstruct(Stack stack, SharedConstructs dependencyInfo, EnvironmentDetails envDetails,
            string buildNumber)
        {
            var idName = $"{envDetails.AppPrefix}-fg-AspNetCore-{envDetails.EnvSuffix}";

            var envName = envDetails.EnvSuffix;

            var vpcId = $"fgvpc";
            var clusterId = $"fgcluster";

            var zoneName = "ibotsota.net";
            var domainName = $"fg-{envName}.{zoneName}";
            var fargateIdName = $"{envDetails.AppPrefix}-fg-{envDetails.EnvSuffix}";

            var vpc = new Vpc(stack, vpcId, new VpcProps
            {
                MaxAzs = 2, // Default is all AZs in region
                NatGateways = 0,
            });

            var cluster = new Cluster(stack, clusterId, new ClusterProps
            {
                Vpc = vpc,
                EnableFargateCapacityProviders = true,
                ContainerInsights = true,
            });
            string hostedZoneID = Constant.HostedZoneID;
            var hostedZone = HostedZone.FromHostedZoneAttributes(stack, $"hostedzone-{zoneName}-fargate",
                new HostedZoneAttributes()
                {
                    HostedZoneId = hostedZoneID,
                    ZoneName = zoneName
                });

            var fargateService = new ApplicationLoadBalancedFargateService(stack, fargateIdName,
                new ApplicationLoadBalancedFargateServiceProps
                {
                    Cluster = cluster,          // Required
                    DesiredCount = 1,           // Default is 1
                    TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                    {
                        Image = ContainerImage.FromAsset(@"..\..\departdir"),
                    },
                    MemoryLimitMiB = 1024,      
                    Cpu = 256,
                    PublicLoadBalancer = true,    // Default is false
                    DomainZone = hostedZone,
                    Protocol = ApplicationProtocol.HTTPS,
                    Certificate = new DnsValidatedCertificate(stack, $"cert-ibotsota-{envName}-fargate", new DnsValidatedCertificateProps()
                    {
                        DomainName = domainName,
                        HostedZone = hostedZone
                    }),
                    TargetProtocol = ApplicationProtocol.HTTP,
                    ServiceName = fargateIdName,
                    AssignPublicIp = true,
                }
            );

            var managedPolicyID = $"{idName}-Policy";

            fargateService.TaskDefinition.TaskRole.AddManagedPolicy(ManagedPolicy.FromManagedPolicyArn(stack, managedPolicyID, "arn:aws:iam::394301006475:policy/iBotSotA-OperatorPolicy"));

            var cNameID = $"cname-{domainName}";
            var route53 = new CnameRecord(stack, cNameID, new CnameRecordProps()
            {
                RecordName = domainName,
                Ttl = Duration.Minutes(5),
                DomainName = fargateService.LoadBalancer.LoadBalancerDnsName,
                Zone = hostedZone,
                Comment = "CloudFormation Stack maintained"
            });

            return fargateService;
        }

        public static FargateService Setup(Stack stack, SharedConstructs sharedConstructs, EnvironmentDetails envDetails,
            string buildNumber)
        {
            var idName = $"{envDetails.AppPrefix}-fg-AspNetCore-{envDetails.EnvSuffix}";

            var envName = envDetails.EnvSuffix;

            //buildNumber ??= Constant.CustomBuildNumber;

            /*
            var vpcId = $"fgvpc";
            var clusterId = $"fgcluster";

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
            */

            var vpc = sharedConstructs.Vpc;
            var cluster = sharedConstructs.Cluster;
            var hostedZone = sharedConstructs.HostedZone;

            var zoneName = hostedZone.ZoneName;
            var fargateIdName = $"{envDetails.AppPrefix}-fg-{envDetails.EnvSuffix}";

            // Create a public IP Fargate service and make it public
            string fargateTaskId = "fg-task";
            var fargateService = new FargateService(stack, fargateIdName, new FargateServiceProps()
            {
                Cluster = cluster,          // Required
                DesiredCount = 1,           // Default is 1
                TaskDefinition = new FargateTaskDefinition(stack, fargateTaskId, new FargateTaskDefinitionProps()
                {
                    Cpu = 256,
                    MemoryLimitMiB = 1024,
                }),
                ServiceName = fargateIdName,
                AssignPublicIp = true,
                CapacityProviderStrategies = new ICapacityProviderStrategy[]
                {
                    new CapacityProviderStrategy()
                    {
                        Base = 1,
                        CapacityProvider = "FARGATE_SPOT",
                        Weight = 1
                    }
                },
            });
            string containerId = "web";
            var container = fargateService.TaskDefinition.AddContainer(containerId, new ContainerDefinitionOptions()
            {
                Image = ContainerImage.FromAsset(@"..\..\departdir"),
                Logging = LogDriver.AwsLogs(new AwsLogDriverProps()
                {
                    LogGroup = new LogGroup(stack, $"fg-{envDetails.EnvSuffix}-log", new LogGroupProps()
                    {
                        LogGroupName = $"/aws/fargate/{envDetails.AppPrefix}-{envDetails.EnvSuffix}",
                        Retention = RetentionDays.ONE_WEEK
                    }),
                    Mode = AwsLogDriverMode.NON_BLOCKING,
                    StreamPrefix = $"{envDetails.AppPrefix}-{envDetails.EnvSuffix}"
                })
            });

            container.AddPortMappings(new PortMapping()
            {
                Protocol = Protocol.TCP,
                ContainerPort = 80
            });

            var targetGroup = sharedConstructs.TargetGroupDict[envName];

            targetGroup.AddTarget(fargateService);

            var managedPolicyID = $"{idName}-Policy";

            fargateService.TaskDefinition.TaskRole.AddManagedPolicy(ManagedPolicy.FromManagedPolicyArn(stack, managedPolicyID, "arn:aws:iam::394301006475:policy/iBotSotA-OperatorPolicy"));

            return fargateService;
        }

    }
}