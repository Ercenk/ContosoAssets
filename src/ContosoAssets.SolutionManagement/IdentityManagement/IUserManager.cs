using System.Threading.Tasks;

namespace ContosoAssets.SolutionManagement.IdentityManagement
{
    public interface IUserManagerAdapter
    {
        Task<bool> DeleteUserAsync(string userName);

        Task<bool> IsUserInRole(string userName, string roleName);
    }
}
