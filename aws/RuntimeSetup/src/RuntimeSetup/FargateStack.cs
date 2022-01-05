using System.Collections.Generic;
using Amazon.CDK;
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

            /*
            if (buildNumber?.StartsWith(envName) == false)
            {
                throw new ArgumentException($"Build number '{buildNumber}' does not match with environment '{envName}'");
            }
            */

            buildNumber ??= "BUILD_NUMBER";

            //var lambdaPackageKey = $"{envName}/iBotSotALambda_{buildNumber}.zip";

            //var vpcId = $"{envName}-ibotsota-fargatevpc";
            //var clusterId = $"{envName}-ibotsota-fargatecluster";
            var vpcId = $"fgvpc";
            var clusterId = $"fgcluster";
            /*
            var function = new Function(stack, idName, new FunctionProps()
            {
                Environment = new Dictionary<string, string>()
                {
                    { "Environment", envDetails.EnvSuffix }
                },
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromBucket(bucket, lambdaPackageKey),
                Handler = "iBotSotALambda::iBotSotALambda.LambdaEntryPoint::FunctionHandlerAsync",
                Tracing = Tracing.ACTIVE,
                Timeout = Duration.Minutes(1),
                MemorySize = 1024
            });
            var managedPolcyID = $"{idName}-Policy";
            function.Role.AddManagedPolicy(ManagedPolicy.FromManagedPolicyArn(stack, managedPolcyID, "arn:aws:iam::394301006475:policy/iBotSotA-OperatorPolicy"));
            */
            //return function;

            var vpc = new Vpc(stack, vpcId, new VpcProps
            {
                MaxAzs = 3, // Default is all AZs in region
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

            var fargatePackagePath = $@"depart\iBotSotALambda_{buildNumber}.zip";


            string hostedZoneID = Constant.HostedZoneID;
            var hostedZone = HostedZone.FromHostedZoneAttributes(stack, $"hostedzone-{zoneName}-fargate",
                new HostedZoneAttributes()
                {
                    HostedZoneId = hostedZoneID,
                    ZoneName = zoneName
                });

            // Create a load-balanced Fargate service and make it public
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


    }
}