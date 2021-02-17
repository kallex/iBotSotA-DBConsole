using System.Runtime.CompilerServices;
using Amazon.CDK;
using Amazon.CDK.AWS.ResourceGroups;

namespace InfraSetup
{
    public class ResourceGroupStack
    {
        public static void Setup(Stack stack, EnvironmentDetails envDetails)
        {
            var idName = $"{envDetails.AppPrefix}-resourceGrp-{envDetails.EnvSuffix}";
            var resourceGroup = new CfnGroup(stack, idName, new CfnGroupProps()
            {
                Name = idName,
            });
        }
    }
}