using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Helper
{
    public class OtpService
    {
        private readonly ILogger<TokenService> _logger;
        private static readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> _otpStorage = new();
        public OtpService(ILogger<TokenService> logger)
        {
            _logger = logger;
        }
        public string GenerateOtp()
        {
            _logger.LogInformation("Generating OTP.");
            byte[] otpBytes = new byte[4];
            RandomNumberGenerator.Fill(otpBytes);
            int otp = BitConverter.ToUInt16(otpBytes, 0) % 1000000;
            string otpString = otp.ToString("D6");
            _logger.LogInformation("Generated OTP: {Otp}", otpString);
            return otpString;
        }

        public void StoreOtp(string email, string otp)
        {
            _logger.LogInformation("Storing OTP for {Email}", email);
            _otpStorage[email] = (otp, DateTime.UtcNow.AddMinutes(5));
        }

        public bool ValidateOtp(string email, string enteredOtp)
        {
            _logger.LogInformation("Validating OTP for {Email}", email);
            if (_otpStorage.TryGetValue(email, out var otpData))
            {
                var (storedOtp, expiry) = otpData;
                if (DateTime.UtcNow <= expiry && storedOtp == enteredOtp)
                {
                    _logger.LogInformation("OTP validation successful for {Email}", email);
                    RemoveOtp(email);
                    return true;
                }
                _logger.LogWarning("Invalid OTP entered or OTP expired for {Email}", email);
                RemoveOtp(email);
            }
            return false;
        }

        public void RemoveOtp(string email)
        {
            _logger.LogInformation("Removing OTP for {Email}", email);
            _otpStorage.TryRemove(email, out _);
        }
    }
}
