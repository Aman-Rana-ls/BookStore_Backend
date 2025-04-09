using RepoLayer.Entity;
using ModelLayer;

namespace BusinessLayer.Interfaces
{
    public interface IUserBL
    {
        User Register(InputModel user);
        string Login(string email, string password);
        Task<bool> ForgetPasswordAsync(string email);
        bool ResetPasswordWithOtp(string email, string otp, string newPassword);
    }
}
