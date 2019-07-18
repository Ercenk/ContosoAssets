using System.Threading.Tasks;

namespace ContosoAssets.WebApp
{
    public interface IUserManagerAdapter
    {
        Task<bool> DeleteUserAsync(string userName);

        Task<bool> IsUserInRole(string userName, string roleName);
    }
}
