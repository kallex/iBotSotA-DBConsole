using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Lambda;

namespace RuntimeSetup
{
    public static class APIGatewayStack
    {
        public static RestApi Setup(Stack stack, EnvironmentDetails envDetails, Function lambdaFunction)
        {
            var apiIdName = $"{envDetails.AppPrefix}-api-{envDetails.EnvSuffix}";
            var api = new LambdaRestApi(stack, apiIdName, new LambdaRestApiProps()
            {
                Handler = lambdaFunction,
            });

            return api;
        }
    }
}