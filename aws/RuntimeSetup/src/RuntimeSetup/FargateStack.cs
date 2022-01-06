using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.AutoScaling;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.S3;

namespace RuntimeSetup
{
    public class FargateStack
    {

        public static ApplicationLoadBalancedFargateService Setup(Stack stack, StackDependency dependencyInfo, EnvironmentDetails envDetails,
            string buildNumber)
        {
            var idName = $"{envDetails.AppPrefix}-fg-AspNetCore-{envDetails.EnvSuffix}";

            var envName = envDetails.EnvSuffix;

            var vpcId = $"fgvpc";
            var clusterId = $"fgcluster";

            var zoneName = "ibotsota.net";
            var domainName = $"fg-{envName}.{zoneName}";
            var fargateIdName = $"{envDetails.AppPrefix}-fg-{envDetails.EnvSuffix}";

            var capacityProvider = new CapacityProviderStrategy()
            {
                CapacityProvider = "FARGATE_SPOT",
            };


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

        public static FargateService SetupFG(Stack stack, StackDependency dependencyInfo, EnvironmentDetails envDetails,
            string buildNumber)
        {
            var idName = $"{envDetails.AppPrefix}-fg-AspNetCore-{envDetails.EnvSuffix}";

            var envName = envDetails.EnvSuffix;

            buildNumber ??= "BUILD_NUMBER";

            var vpcId = $"fgvpc";
            var clusterId = $"fgcluster";

            var vpc = new Vpc(stack, vpcId, new VpcProps
            {
                MaxAzs = 1, // Default is all AZs in region
                NatGateways = 0
            });

            var cluster = new Cluster(stack, clusterId, new ClusterProps
            {
                Vpc = vpc,
                EnableFargateCapacityProviders = true,
                ContainerInsights = true,
            });

            var zoneName = "ibotsota.net";
            var domainName = $"fg-{envName}.{zoneName}";
            var fargateIdName = $"{envDetails.AppPrefix}-fg-{envDetails.EnvSuffix}";

            string hostedZoneID = Constant.HostedZoneID;
            var hostedZone = HostedZone.FromHostedZoneAttributes(stack, $"hostedzone-{zoneName}-fargate",
                new HostedZoneAttributes()
                {
                    HostedZoneId = hostedZoneID,
                    ZoneName = zoneName
                });

            // Create a public IP Fargate service and make it public
            string fgTaskId = "fgTask";
            var fgService = new FargateService(stack, fargateIdName, new FargateServiceProps()
            {
                AssignPublicIp = false,
                Cluster = cluster,
                DesiredCount = 1,
                TaskDefinition = new FargateTaskDefinition(stack, fgTaskId, new FargateTaskDefinitionProps()
                {

                }),


            });

            string containerId = "fgContainer";
            fgService.TaskDefinition.AddContainer(containerId, new ContainerDefinitionOptions()
            {
                Image = ContainerImage.FromAsset(@"..\..\departdir"),
            });



            /*
            var fargateService = new ApplicationLoadBalancedFargateService(stack, fargateIdName,
                new ApplicationLoadBalancedFargateServiceProps
                {
                    Cluster = cluster,          // Required
                    DesiredCount = 1,           // Default is 1
                    TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                    {
                        Image = ContainerImage.FromAsset(@"..\..\departdir"),
                        //Image = ContainerImage.FromAsset(@"D:\UserData\Kalle\work\iBotSotA-DataService\src\iBotSotALambda\bin\Debug\netcoreapp3.1"), //ContainerImage.FromRegistry("amazon/amazon-ecs-sample")
                        //ContainerPort = 5000
                    },
                    MemoryLimitMiB = 512,
                    Cpu = 256,
                    PublicLoadBalancer = true,    // Default is false
                    DomainZone = hostedZone,
                    Certificate = new DnsValidatedCertificate(stack, $"cert-ibotsota-{envName}-fargate", new DnsValidatedCertificateProps()
                    {
                        DomainName = domainName,
                        HostedZone = hostedZone
                    }),
                    TargetProtocol = ApplicationProtocol.HTTP,
                    //ProtocolVersion = ApplicationProtocolVersion.HTTP2,
                    ServiceName = fargateIdName,
                }
            );
            */
            var managedPolicyID = $"{idName}-Policy";

            fgService.TaskDefinition.TaskRole.AddManagedPolicy(ManagedPolicy.FromManagedPolicyArn(stack, managedPolicyID, "arn:aws:iam::394301006475:policy/iBotSotA-OperatorPolicy"));

            /*
            var cNameID = $"cname-{domainName}";
            var route53 = new ARecord(stack, cNameID, new 
                ARecordProps()
            {
                RecordName = domainName,
                Ttl = Duration.Minutes(5),
                Zone = hostedZone,
                Comment = "CloudFormation Stack maintained"
            });
            */
            return fgService;
        }

    }
}