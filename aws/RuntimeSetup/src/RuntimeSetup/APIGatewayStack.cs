using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.SSM;

namespace RuntimeSetup
{
    public static class APIGatewayStack
    {
        public static RestApi Setup(Stack stack, EnvironmentDetails envDetails, Function lambdaFunction)
        {
            var zoneName = "theball.me";
            var envName = envDetails.EnvSuffix;
            var domainName = $"lambda-{envName}-ibotsota.{zoneName}";
            var apiIdName = $"{envDetails.AppPrefix}-api-{envDetails.EnvSuffix}";
            string hostedZoneID = "Z1XC67ZO4ST4EW";
            var hostedZone = HostedZone.FromHostedZoneAttributes(stack, $"hostedzone-{zoneName}",
                new HostedZoneAttributes()
                {
                    HostedZoneId = hostedZoneID,
                    ZoneName = zoneName
                });
            var api = new LambdaRestApi(stack, apiIdName, new LambdaRestApiProps()
            {
                Handler = lambdaFunction,
                DeployOptions = new StageOptions()
                {
                    StageName = envName,
                    TracingEnabled = true,
                },
                DomainName = new DomainNameOptions()
                {
                    DomainName = domainName,
                    Certificate = new DnsValidatedCertificate(stack, $"cert-ibotsota-{envName}", new DnsValidatedCertificateProps()
                        {
                            DomainName = domainName,
                            HostedZone = hostedZone
                        })
                },
            });

            var cNameID = $"cname-{domainName}";
            var route53 = new CnameRecord(stack, cNameID, new CnameRecordProps()
            {
                RecordName = domainName,
                Ttl = Duration.Minutes(5),
                DomainName = api.DomainName.DomainNameAliasDomainName,
                Zone = hostedZone,
                Comment = "CloudFormation Stack maintained"
            });

            return api;
        }
    }
}