using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interfaces;
using ModelLayer;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace BookStoreBackEnd.Controllers
{
    [Route("/adminuser")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserBL _adminBL;
        private readonly ILogger<AdminUserController> _logger;

        public AdminUserController(IAdminUserBL adminBL, ILogger<AdminUserController> logger)
        {
            _adminBL = adminBL;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new admin user in the system.
        /// </summary>
        /// <param name="inputModel">Admin user registration details</param>
        /// <returns>A response indicating the result of the registration process</returns>
        [HttpPost("")]
        public IActionResult Register(InputModel inputModel)
        {
            try
            {
                _logger.LogInformation("Attempting to register admin user.");
                var result = _adminBL.Register(inputModel);

                if (result != null)
                {
                    _logger.LogInformation("Admin user registration successful.");
                    return Ok(new ResponseModel<object>
                    {
                        Success = true,
                        Message = "AdminUser Registration Successful",
                        Data = result
                    });
                }

                _logger.LogWarning("Admin registration failed.");
                return BadRequest(new ResponseModel<object>
                {
                    Success = false,
                    Message = "Admin registration failed",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during admin registration.");
                return StatusCode(500, new ResponseModel<object>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Authenticates an admin user and returns a JWT token.
        /// </summary>
        /// <param name="email">Admin email</param>
        /// <param name="password">Admin password</param>
        /// <returns>JWT token if authentication is successful</returns>
        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            try
            {
                _logger.LogInformation("Admin user attempting login.");
                var token = _adminBL.Login(email, password);

                if (!string.IsNullOrEmpty(token))
                {
                    _logger.LogInformation("Admin login successful.");
                    return Ok(new ResponseModel<string>
                    {
                        Success = true,
                        Message = "Login Successful",
                        Data = token
                    });
                }

                _logger.LogWarning("Invalid login attempt.");
                return Unauthorized(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Invalid credentials",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during admin login.");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Sends an OTP to the admin's registered email for password reset.
        /// </summary>
        /// <param name="email">Admin user's email address</param>
        /// <returns>A response indicating whether the OTP was sent successfully</returns>
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPasswordAsync(string email)
        {
            try
            {
                _logger.LogInformation("Request to send OTP for password reset.");
                bool result = await _adminBL.ForgetPasswordAsync(email);

                if (result)
                {
                    _logger.LogInformation("OTP sent successfully.");
                    return Ok(new ResponseModel<bool>
                    {
                        Success = true,
                        Message = "OTP sent successfully",
                        Data = true
                    });
                }

                _logger.LogWarning("Failed to send OTP.");
                return BadRequest(new ResponseModel<bool>
                {
                    Success = false,
                    Message = "Failed to send OTP",
                    Data = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending OTP.");
                return StatusCode(500, new ResponseModel<bool>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = false
                });
            }
        }

        /// <summary>
        /// Resets admin password using the OTP sent to email.
        /// </summary>
        /// <param name="email">Admin user's email address</param>
        /// <param name="otp">One-time password</param>
        /// <param name="newPassword">New password to be set</param>
        /// <returns>A response indicating whether the password was reset successfully</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordWithOtpAsync(string email, string otp, string newPassword)
        {
            try
            {
                _logger.LogInformation("Request to reset password using OTP.");
                bool result = await _adminBL.ResetPasswordWithOtpAsync(email, otp, newPassword);

                if (result)
                {
                    _logger.LogInformation("Password reset successfully.");
                    return Ok(new ResponseModel<bool>
                    {
                        Success = true,
                        Message = "Password reset successfully",
                        Data = true
                    });
                }

                _logger.LogWarning("Invalid OTP or password reset request failed.");
                return BadRequest(new ResponseModel<bool>
                {
                    Success = false,
                    Message = "Invalid OTP or request",
                    Data = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password reset.");
                return StatusCode(500, new ResponseModel<bool>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = false
                });
            }
        }
    }
}
