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
        public class UserControllerTests
        {
            private Mock<IUserBL> _mockUserBL;
            private Mock<ILogger<UserController>> _mockLogger;
            private UserController _controller;

            [SetUp]
            public void Setup()
            {
                _mockUserBL = new Mock<IUserBL>();
                _mockLogger = new Mock<ILogger<UserController>>();
                _controller = new UserController(_mockUserBL.Object, _mockLogger.Object);
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

                var user = new User
                {
                    Id = 1,
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    Email = input.Email
                };

                _mockUserBL.Setup(bl => bl.Register(input)).Returns(user);

                var result = _controller.Register(input) as OkObjectResult;

                Assert.That(result, Is.Not.Null);
                var response = result.Value as ResponseModel<object>;
                Assert.That(response.Success, Is.True);
                Assert.That(response.Message, Is.EqualTo("User Registration Successful"));
                Assert.That(response.Data, Is.EqualTo(user));
            }

            [Test]
            public void Login_ReturnsOk_WhenSuccessful()
            {
                string email = "test@example.com";
                string password = "password123";
                string fakeToken = "fake-jwt-token";

                _mockUserBL.Setup(bl => bl.Login(email, password)).Returns(fakeToken);

                var result = _controller.Login(email, password) as OkObjectResult;

                Assert.That(result, Is.Not.Null);
                var response = result.Value as ResponseModel<string>;
                Assert.That(response.Success, Is.True);
                Assert.That(response.Data, Is.EqualTo(fakeToken));
            }

            [Test]
            public void Login_ReturnsUnauthorized_WhenInvalid()
            {
                string email = "test@example.com";
                string password = "wrongpassword";

                _mockUserBL.Setup(bl => bl.Login(email, password)).Returns(string.Empty);

                var result = _controller.Login(email, password) as UnauthorizedObjectResult;

                Assert.That(result, Is.Not.Null);
                var response = result.Value as ResponseModel<string>;
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo("Invalid credentials"));
            }

            [Test]
            public async Task ForgetPassword_ReturnsOk_WhenSuccessful()
            {
                string email = "test@example.com";
                _mockUserBL.Setup(bl => bl.ForgetPasswordAsync(email)).ReturnsAsync(true);

                var result = await _controller.ForgetPassword(email) as OkObjectResult;

                Assert.That(result, Is.Not.Null);
                var response = result.Value as ResponseModel<bool>;
                Assert.That(response.Success, Is.True);
                Assert.That(response.Message, Is.EqualTo("OTP sent successfully"));
            }

            [Test]
            public async Task ForgetPassword_ReturnsBadRequest_WhenFails()
            {
                string email = "test@example.com";
                _mockUserBL.Setup(bl => bl.ForgetPasswordAsync(email)).ReturnsAsync(false);

                var result = await _controller.ForgetPassword(email) as BadRequestObjectResult;


                Assert.That(result, Is.Not.Null);
                var response = result.Value as ResponseModel<bool>;
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo("Failed to send OTP"));
            }

            [Test]
            public void ResetPasswordWithOtp_ReturnsOk_WhenSuccessful()
            {
                string email = "test@example.com";
                string otp = "123456";
                string newPassword = "newPassword123";

                _mockUserBL.Setup(bl => bl.ResetPasswordWithOtp(email, otp, newPassword)).Returns(true);

                var result = _controller.ResetPasswordWithOtp(email, otp, newPassword) as OkObjectResult;

                Assert.That(result, Is.Not.Null);
                var response = result.Value as ResponseModel<bool>;
                Assert.That(response.Success, Is.True);
                Assert.That(response.Message, Is.EqualTo("Password reset successfully"));
            }

            [Test]
            public void ResetPasswordWithOtp_ReturnsBadRequest_WhenFails()
            {
          
                string email = "test@example.com";
                string otp = "wrongOTP";
                string newPassword = "newPassword123";

                _mockUserBL.Setup(bl => bl.ResetPasswordWithOtp(email, otp, newPassword)).Returns(false);

                var result = _controller.ResetPasswordWithOtp(email, otp, newPassword) as BadRequestObjectResult;

                Assert.That(result, Is.Not.Null);
                var response = result.Value as ResponseModel<bool>;
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo("Invalid OTP or request"));
            }
        }
    }
