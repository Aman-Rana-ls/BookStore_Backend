using BusinessLayer.Interfaces;
using RepoLayer.Interfaces;
using RepoLayer.Entity;
using ModelLayer;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using BusinessLayer.Helper;

namespace BusinessLayer.Services
{
    public class AdminUserBL : IAdminUserBL
    {
        private readonly IAdminUserRL _adminRL;
        private readonly TokenService _tokenService;
        private readonly EmailService _emailService;
        private readonly OtpService _otpService;
        private readonly ILogger<AdminUserBL> _logger;

        public AdminUserBL(IAdminUserRL adminRL, TokenService tokenService, EmailService emailService, OtpService otpService,ILogger<AdminUserBL> logger)
        {
            _adminRL = adminRL;
            _tokenService = tokenService;
            _emailService = emailService;
            _otpService = otpService;
            _logger = logger;
        }

        public AdminUser Register(InputModel inputModel)
        {
            try
            {
                _logger.LogInformation("Registering new admin user: {Email}", inputModel.Email);
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(inputModel.Password);
                var adminUser = new AdminUser
                {
                    FirstName = inputModel.FirstName,
                    LastName = inputModel.LastName,
                    Email = inputModel.Email,
                    PasswordHash = hashedPassword,
                    Role = "Admin"
                };
                var registeredUser = _adminRL.Register(adminUser);
                _logger.LogInformation("Admin user registered successfully: {Email}", inputModel.Email);
                return registeredUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering admin user: {Email}", inputModel.Email);
                throw;
            }
        }

        public string Login(string email, string password)
        {
            try
            {
                _logger.LogInformation("Admin login attempt: {Email}", email);
                var adminUser = _adminRL.Login(email);
                if (adminUser != null && BCrypt.Net.BCrypt.Verify(password, adminUser.PasswordHash))
                {
                    _logger.LogInformation("Login successful for admin: {Email}", email);
                    return _tokenService.GenerateJwtToken(adminUser.Id, adminUser.Email, adminUser.Role);
                }
                _logger.LogWarning("Login failed for admin: {Email}", email);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin login: {Email}", email);
                throw;
            }
        }

        public async Task<bool> ForgetPasswordAsync(string email)
        {
            try
            {
                _logger.LogInformation("Sending OTP for password reset to: {Email}", email);
                var adminUser = _adminRL.Login(email);
                if (adminUser == null)
                {
                    _logger.LogWarning("Admin user not found: {Email}", email);
                    return false;
                }

                string otp = _otpService.GenerateOtp();
                _otpService.StoreOtp(email, otp);
                bool emailSent = await _emailService.SendOtpEmailAsync(email, otp);

                if (emailSent)
                {
                    _logger.LogInformation("OTP email sent successfully to: {Email}", email);
                }
                else
                {
                    _logger.LogError("Failed to send OTP email to: {Email}", email);
                }

                return emailSent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP for admin: {Email}", email);
                throw;
            }
        }

        public async Task<bool> ResetPasswordWithOtpAsync(string email, string otp, string newPassword)
        {
            try
            {
                _logger.LogInformation("Resetting password for admin: {Email}", email);
                if (_otpService.ValidateOtp(email, otp))
                {
                    var adminUser = _adminRL.GetAdminByEmail(email);
                    if (adminUser != null)
                    {
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                        _adminRL.UpdateAdminPassword(email, hashedPassword);
                        _otpService.RemoveOtp(email);
                        _logger.LogInformation("Password reset successful for admin: {Email}", email);
                        return true;
                    }
                }
                _logger.LogWarning("Invalid OTP for password reset: {Email}", email);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for admin: {Email}", email);
                throw;
            }
        }
    }
}