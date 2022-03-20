using Amazon.CDK;
using Amazon.CDK.AWS.XRay;

namespace RuntimeSetup
{
    public class XRayStack
    {
        public static CfnSamplingRule Setup(Stack stack, EnvironmentDetails envDetails)
        {
            var samplingRuleId = $"{envDetails.AppPrefix}-samplingrule-{envDetails.EnvSuffix}";
            var hostName = $"{envDetails.EnvSuffix}.{envDetails.AppPrefix}.net".ToLower();
            var ruleName = $"{envDetails.AppPrefix}-API-{envDetails.EnvSuffix}";
            CfnSamplingRule samplingRule = new CfnSamplingRule(stack, samplingRuleId, new CfnSamplingRuleProps()
            {
                RuleName = ruleName,
                SamplingRule = new CfnSamplingRule.SamplingRuleProperty()
                {
                    ServiceName = envDetails.AppPrefix,
                    ServiceType = "*",
                    HttpMethod = "*",
                    UrlPath = "*",
                    ResourceArn = "*",
                    RuleName = ruleName,
                    Host = hostName,
                    ReservoirSize = 12,
                    FixedRate = 0.05,
                    Priority = 111,
                    Version = 1,
                }
            });
            return samplingRule; 
        }
    }
}