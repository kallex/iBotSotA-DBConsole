using System.Runtime.CompilerServices;
using Amazon.CDK;
using Amazon.CDK.AWS.ResourceGroups;

namespace InfraSetup
{
    public class ResourceGroupStack
    {
        public static void Setup(Stack stack)
        {
            var resourceGroup = new CfnGroup(stack, "ibotsota-resourcegroup", new CfnGroupProps()
            {
                Name = "ibotsota-resourcegroup",
            });
        }
    }
}