using RepoLayer.Entity;

namespace RepoLayer.Interfaces
{
    public interface IUserRL
    {
        User Register(User user);
        User Login(string email);
        User GetUserByEmail(string email);
        void UpdatePassword(string email, string hashedPassword);
    }
}
