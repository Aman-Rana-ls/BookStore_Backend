using RepoLayer.Entity;
using ModelLayer;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IAdminUserBL
    {
        AdminUser Register(InputModel adminUser);
        string Login(string email, string password);
        Task<bool> ForgetPasswordAsync(string email);
        Task<bool> ResetPasswordWithOtpAsync(string email, string otp, string newPassword);
    }
}
