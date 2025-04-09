using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interfaces;
using ModelLayer;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace BookStoreBackEnd.Controllers
{
    /// <summary>
    /// Controller for handling user authentication and account management operations
    /// </summary>
    [Route("/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Initializes a new instance of the UserController
        /// </summary>
        /// <param name="userBL">Business layer service for user operations</param>
        /// <param name="logger">Logger for logging information</param>
        public UserController(IUserBL userBL, ILogger<UserController> logger)
        {
            _userBL = userBL;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user account
        /// </summary>
        [HttpPost("")]
        public IActionResult Register([FromBody] InputModel inputModel)
        {
            try
            {
                _logger.LogInformation("Registering a new user");
                var result = _userBL.Register(inputModel);

                if (result != null)
                {
                    return Ok(new ResponseModel<object>
                    {
                        Success = true,
                        Message = "User Registration Successful",
                        Data = result
                    });
                }

                return BadRequest(new ResponseModel<object>
                {
                    Success = false,
                    Message = "User registration failed",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, new ResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred during registration",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Authenticates a user and generates an access token
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            try
            {
                _logger.LogInformation("User attempting to log in");
                var token = _userBL.Login(email, password);

                if (!string.IsNullOrEmpty(token))
                {
                    return Ok(new ResponseModel<string>
                    {
                        Success = true,
                        Message = "Login Successful",
                        Data = token
                    });
                }

                return Unauthorized(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Invalid credentials",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred during login",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Initiates password reset process by sending OTP to user's email
        /// </summary>
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                _logger.LogInformation("Sending OTP for password reset");
                bool result = await _userBL.ForgetPasswordAsync(email);

                if (result)
                {
                    return Ok(new ResponseModel<bool>
                    {
                        Success = true,
                        Message = "OTP sent successfully",
                        Data = true
                    });
                }

                return BadRequest(new ResponseModel<bool>
                {
                    Success = false,
                    Message = "Failed to send OTP",
                    Data = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP");
                return StatusCode(500, new ResponseModel<bool>
                {
                    Success = false,
                    Message = "An error occurred while sending OTP",
                    Data = false
                });
            }
        }

        /// <summary>
        /// Resets user password using the OTP received via email
        /// </summary>
        [HttpPost("reset-password")]
        public IActionResult ResetPasswordWithOtp(string email, string otp, string newPassword)
        {
            try
            {
                _logger.LogInformation("Resetting password with OTP");
                bool result = _userBL.ResetPasswordWithOtp(email, otp, newPassword);

                if (result)
                {
                    return Ok(new ResponseModel<bool>
                    {
                        Success = true,
                        Message = "Password reset successfully",
                        Data = true
                    });
                }

                return BadRequest(new ResponseModel<bool>
                {
                    Success = false,
                    Message = "Invalid OTP or request",
                    Data = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return StatusCode(500, new ResponseModel<bool>
                {
                    Success = false,
                    Message = "An error occurred while resetting password",
                    Data = false
                });
            }
        }
    }
}
