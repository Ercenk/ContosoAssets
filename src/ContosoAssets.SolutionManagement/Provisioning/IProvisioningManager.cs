using System;
using System.Threading.Tasks;

namespace ContosoAssets.SolutionManagement.Provisioning
{
    public interface IProvisioningManager
    {
        Task<OperationResult> AddEnvironmentAsync(string environmentName, Guid skuId, string region);

        Task<OperationResult> RemoveEnvironmentAsync(string environmentName);
    }
}
