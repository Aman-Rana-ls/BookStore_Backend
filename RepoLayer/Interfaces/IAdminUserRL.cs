using RepoLayer.Entity;

namespace RepoLayer.Interfaces
{
    public interface IAdminUserRL
    {
        AdminUser Register(AdminUser adminUser);
        AdminUser Login(string email);
        AdminUser GetAdminByEmail(string email);
        void UpdateAdminPassword(string email, string hashedPassword);
    }
}
