using System;
using System.Threading.Tasks;

namespace ContosoAssets.SolutionManagement.Provisioning
{
    // Does nothing for now. It is here for demonstration purposes.
    public class ProvisioningManager : IProvisioningManager
    {
        public Task<OperationResult> AddEnvironmentAsync(string environmentName, Guid skuId, string region)
        {
            return Task.FromResult(new OperationResult());
        }

        public Task<OperationResult> RemoveEnvironmentAsync(string environmentName)
        {
            return Task.FromResult(new OperationResult());
        }
    }
}
