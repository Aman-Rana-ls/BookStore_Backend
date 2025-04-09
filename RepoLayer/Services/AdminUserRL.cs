using Microsoft.Extensions.Logging;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System.Linq;

namespace RepoLayer.Services
{
    public class AdminUserRL : IAdminUserRL
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminUserRL> _logger;

        public AdminUserRL(AppDbContext context, ILogger<AdminUserRL> logger)
        {
            _context = context;
            _logger = logger;
        }

        public AdminUser Register(AdminUser adminUser)
        {
            _logger.LogInformation("Registering new admin with email {Email}", adminUser.Email);
            _context.AdminUsers.Add(adminUser);
            _context.SaveChanges();
            return adminUser;
        }

        public AdminUser Login(string email)
        {
            _logger.LogInformation("Admin login attempt with email {Email}", email);
            return _context.AdminUsers.FirstOrDefault(a => a.Email == email);
        }

        public AdminUser GetAdminByEmail(string email)
        {
            _logger.LogInformation("Fetching admin details for email {Email}", email);
            return _context.AdminUsers.FirstOrDefault(a => a.Email == email);
        }

        public void UpdateAdminPassword(string email, string hashedPassword)
        {
            _logger.LogInformation("Updating password for admin with email {Email}", email);
            var admin = _context.AdminUsers.FirstOrDefault(a => a.Email == email);
            if (admin != null)
            {
                admin.PasswordHash = hashedPassword;
                _context.SaveChanges();
            }
        }
    }
}
