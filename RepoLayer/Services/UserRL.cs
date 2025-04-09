using Microsoft.Extensions.Logging;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System.Linq;

namespace RepoLayer.Services
{
    public class UserRL : IUserRL
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRL> _logger;

        public UserRL(AppDbContext context, ILogger<UserRL> logger)
        {
            _context = context;
            _logger = logger;
        }

        public User Register(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                _logger.LogInformation("User registered successfully with email: {Email}", user.Email);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user with email: {Email}", user.Email);
                throw;
            }
        }

        public User Login(string email)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                    _logger.LogInformation("User login attempt successful for email: {Email}", email);
                else
                    _logger.LogWarning("User login attempt failed for email: {Email}", email);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for email: {Email}", email);
                throw;
            }
        }

        public User GetUserByEmail(string email)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                    _logger.LogInformation("User retrieved with email: {Email}", email);
                else
                    _logger.LogWarning("No user found with email: {Email}", email);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by email: {Email}", email);
                throw;
            }
        }

        public void UpdatePassword(string email, string hashedPassword)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    user.PasswordHash = hashedPassword;
                    _context.SaveChanges();
                    _logger.LogInformation("Password updated successfully for email: {Email}", email);
                }
                else
                {
                    _logger.LogWarning("Attempted to update password for non-existent email: {Email}", email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password for email: {Email}", email);
                throw;
            }
        }
    }
}
