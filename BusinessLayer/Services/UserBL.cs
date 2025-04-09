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
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly TokenService _tokenService;
        private readonly EmailService _emailService;
        private readonly OtpService _otpService;
        private readonly HashPassword _hashPassword;
        private readonly ILogger<UserBL> _logger;

        public UserBL(IUserRL userRL, TokenService tokenService, EmailService emailService, OtpService otpService,HashPassword hashPassword, ILogger<UserBL> logger)
        {
            _userRL = userRL;
            _tokenService = tokenService;
            _emailService = emailService;
            _otpService = otpService;
            _logger = logger;
            _hashPassword = hashPassword;
        }

        public User Register(InputModel inputModel)
        {
            try
            {
                _logger.LogInformation("Registering new user: {Email}", inputModel.Email);
                var hashedPassword  = _hashPassword.HashThePassword(inputModel.Password);

                var user = new User
                {
                    FirstName = inputModel.FirstName,
                    LastName = inputModel.LastName,
                    Email = inputModel.Email,
                    PasswordHash = hashedPassword,
                    Role = "User"
                };

                var registeredUser = _userRL.Register(user);
                _logger.LogInformation("User registered successfully: {Email}", inputModel.Email);
                return registeredUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user: {Email}", inputModel.Email);
                throw;
            }
        }

        public string Login(string email, string password)
        {
            try
            {
                _logger.LogInformation("User login attempt: {Email}", email);
                var user = _userRL.Login(email);

                if (user != null && _hashPassword.VerifyPassword(password, user.PasswordHash))
                {
                    _logger.LogInformation("Login successful: {Email}", email);
                    return _tokenService.GenerateJwtToken(user.Id, user.Email, user.Role);
                }

                _logger.LogWarning("Login failed for user: {Email}", email);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Email}", email);
                throw;
            }
        }

        public async Task<bool> ForgetPasswordAsync(string email)
        {
            try
            {
                _logger.LogInformation("Initiating password reset for: {Email}", email);
                var user = _userRL.GetUserByEmail(email);

                if (user == null)
                {
                    _logger.LogWarning("User not found: {Email}", email);
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
                _logger.LogError(ex, "Error while sending OTP for: {Email}", email);
                throw;
            }
        }

        public bool ResetPasswordWithOtp(string email, string otp, string newPassword)
        {
            try
            {
                _logger.LogInformation("Attempting password reset for: {Email}", email);

                if (_otpService.ValidateOtp(email, otp))
                {
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    _userRL.UpdatePassword(email, hashedPassword);
                    _otpService.RemoveOtp(email);

                    _logger.LogInformation("Password reset successful for: {Email}", email);
                    return true;
                }

                _logger.LogWarning("Invalid OTP for password reset: {Email}", email);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for: {Email}", email);
                throw;
            }
        }
    }
}
