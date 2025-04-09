using BookStoreBackEnd.Controllers;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ModelLayer;
using NUnit.Framework;
using RepoLayer.Entity;
using System.Threading.Tasks;

namespace BookStoreBackEnd.Tests
{
    public class AdminUserControllerTests
    {
        private Mock<IAdminUserBL> _mockAdminBL;
        private Mock<ILogger<AdminUserController>> _mockLogger;
        private AdminUserController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAdminBL = new Mock<IAdminUserBL>();
            _mockLogger = new Mock<ILogger<AdminUserController>>();
            _controller = new AdminUserController(_mockAdminBL.Object, _mockLogger.Object);
        }

        [Test]
        public void Register_ReturnsOk_WhenSuccessful()
        {
            var input = new InputModel
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password123"
            };

            var resultUser = new AdminUser
            {
                Id = 1,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email
            };

            _mockAdminBL.Setup(bl => bl.Register(input)).Returns(resultUser);

            var result = _controller.Register(input) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<object>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.EqualTo("AdminUser Registration Successful"));
            Assert.That(response.Data, Is.EqualTo(resultUser));
        }

        [Test]
        public void Login_ReturnsOk_WhenSuccessful()
        {
            string email = "admin@example.com";
            string password = "password123";
            string fakeToken = "fake-jwt-token";

            _mockAdminBL.Setup(bl => bl.Login(email, password)).Returns(fakeToken);

            var result = _controller.Login(email, password) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<string>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Data, Is.EqualTo(fakeToken));
        }

        [Test]
        public void Login_ReturnsUnauthorized_WhenInvalid()
        {
            string email = "admin@example.com";
            string password = "wrongpassword";

            _mockAdminBL.Setup(bl => bl.Login(email, password)).Returns(string.Empty);

            var result = _controller.Login(email, password) as UnauthorizedObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<string>;
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("Invalid credentials"));
        }

        [Test]
        public async Task ForgetPasswordAsync_ReturnsOk_WhenSuccessful()
        {
            string email = "admin@example.com";

            _mockAdminBL.Setup(bl => bl.ForgetPasswordAsync(email)).ReturnsAsync(true);

            var result = await _controller.ForgetPasswordAsync(email) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<bool>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.EqualTo("OTP sent successfully"));
        }

        [Test]
        public async Task ForgetPasswordAsync_ReturnsBadRequest_WhenFails()
        {
            string email = "admin@example.com";

            _mockAdminBL.Setup(bl => bl.ForgetPasswordAsync(email)).ReturnsAsync(false);

            var result = await _controller.ForgetPasswordAsync(email) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<bool>;
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("Failed to send OTP"));
        }

        [Test]
        public async Task ResetPasswordWithOtpAsync_ReturnsOk_WhenSuccessful()
        {
            string email = "admin@example.com";
            string otp = "123456";
            string newPassword = "NewPass123";

            _mockAdminBL.Setup(bl => bl.ResetPasswordWithOtpAsync(email, otp, newPassword)).ReturnsAsync(true);

            var result = await _controller.ResetPasswordWithOtpAsync(email, otp, newPassword) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<bool>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.EqualTo("Password reset successfully"));
        }

        [Test]
        public async Task ResetPasswordWithOtpAsync_ReturnsBadRequest_WhenFails()
        {
            string email = "admin@example.com";
            string otp = "wrongotp";
            string newPassword = "NewPass123";

            _mockAdminBL.Setup(bl => bl.ResetPasswordWithOtpAsync(email, otp, newPassword)).ReturnsAsync(false);

            var result = await _controller.ResetPasswordWithOtpAsync(email, otp, newPassword) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<bool>;
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("Invalid OTP or request"));
        }
    }
}
