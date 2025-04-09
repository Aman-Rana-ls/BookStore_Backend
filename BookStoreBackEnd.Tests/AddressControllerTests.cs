using BookStoreBackEnd.Controllers;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer;
using Moq;
using NUnit.Framework;
using RepoLayer.Entity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace BookStoreBackEnd.Tests
{
    public class AddressControllerTests
    {
        private Mock<IAddressBL> _mockAddressBL;
        private Mock<ILogger<AddressController>> _mockLogger;
        private AddressController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAddressBL = new Mock<IAddressBL>();
            _mockLogger = new Mock<ILogger<AddressController>>();
            _controller = new AddressController(_mockAddressBL.Object, _mockLogger.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task AddAddress_ReturnsOk_WhenSuccessful()
        {
            var input = new AddressInputModel
            {
                FullName = "Aman",
                MobileNumber = "9876543210",
                AddressLine = "123 Main St",
                PinCode = "110001",
                City = "Delhi",
                State = "Delhi",
                AddressType = "Home"
            };

            var expectedAddress = new Address
            {
                AddressId = 1,
                FullName = input.FullName,
                MobileNumber = input.MobileNumber,
                AddressLine = input.AddressLine,
                PinCode = input.PinCode,
                City = input.City,
                State = input.State,
                Type = input.AddressType,
                UserId = 1
            };

            _mockAddressBL.Setup(bl => bl.AddAddress(input, 1)).ReturnsAsync(expectedAddress);

            var result = await _controller.AddAddress(input) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<Address>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Data, Is.EqualTo(expectedAddress));
        }

        [Test]
        public async Task AddAddress_ReturnsBadRequest_WhenModelInvalid()
        {
            _controller.ModelState.AddModelError("FullName", "FullName is required");

            var input = new AddressInputModel
            {
                MobileNumber = "9876543210",
                AddressLine = "123 Main St",
                PinCode = "110001",
                City = "Delhi",
                State = "Delhi",
                AddressType = "Home"
            };

            var result = await _controller.AddAddress(input) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task AddAddress_ReturnsUnauthorized_WhenUserIdMissing()
        {
            var controller = new AddressController(_mockAddressBL.Object, _mockLogger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // No claims
            };

            var input = new AddressInputModel
            {
                FullName = "Aman",
                MobileNumber = "9876543210",
                AddressLine = "123 Main St",
                PinCode = "110001",
                City = "Delhi",
                State = "Delhi",
                AddressType = "Home"
            };

            var result = await controller.AddAddress(input) as UnauthorizedObjectResult;

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetAll_ReturnsOk_WithListOfAddresses()
        {
            var addressList = new List<Address> { new Address { AddressId = 1 }, new Address { AddressId = 2 } };
            _mockAddressBL.Setup(bl => bl.GetAllAddresses()).ReturnsAsync(addressList);

            var result = await _controller.GetAll() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<List<Address>>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Data, Is.EqualTo(addressList));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenNotExists()
        {
            _mockAddressBL.Setup(bl => bl.GetAddressById(99)).ReturnsAsync((Address)null);

            var result = await _controller.GetById(99) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<string>;
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("Address not found."));
        }

        [Test]
        public async Task GetByUserId_ReturnsOk_WhenSuccessful()
        {
            int userId = 1;
            var list = new List<Address> { new Address { AddressId = 1, UserId = userId } };

            _mockAddressBL.Setup(bl => bl.GetAddressesByUserId(userId)).ReturnsAsync(list);

            var result = await _controller.GetByUserId(userId) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<List<Address>>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Data, Is.EqualTo(list));
        }

        [Test]
        public async Task Update_ReturnsOk_WhenSuccessful()
        {
            var input = new AddressInputModel
            {
                FullName = "Aman Updated",
                MobileNumber = "9876543210",
                AddressLine = "New Address",
                PinCode = "110002",
                City = "Delhi",
                State = "Delhi",
                AddressType = "Work"
            };

            var updated = new Address
            {
                AddressId = 1,
                FullName = input.FullName,
                AddressLine = input.AddressLine,
                Type = input.AddressType,
                UserId = 1
            };

            _mockAddressBL.Setup(bl => bl.UpdateAddress(input, 1)).ReturnsAsync(updated);

            var result = await _controller.Update(input) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<Address>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Data.FullName, Is.EqualTo("Aman Updated"));
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenUpdateFails()
        {
            var input = new AddressInputModel
            {
                FullName = "Invalid",
                MobileNumber = "0000000000",
                AddressLine = "Nowhere",
                PinCode = "000000",
                City = "Ghost",
                State = "Unknown",
                AddressType = "Other"
            };

            _mockAddressBL.Setup(bl => bl.UpdateAddress(input, 1)).ReturnsAsync((Address)null);

            var result = await _controller.Update(input) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<string>;
            Assert.That(response.Success, Is.False);
        }

        [Test]
        public async Task Delete_ReturnsOk_WhenDeleted()
        {
            _mockAddressBL.Setup(bl => bl.DeleteAddress(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<string>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.EqualTo("Address deleted successfully."));
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenIdInvalid()
        {
            _mockAddressBL.Setup(bl => bl.DeleteAddress(99)).ReturnsAsync(false);

            var result = await _controller.Delete(99) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<string>;
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("Address not found."));
        }

        [Test]
        public async Task AddAddress_ThrowsException_Returns500()
        {
            var input = new AddressInputModel
            {
                FullName = "Aman",
                MobileNumber = "9876543210",
                AddressLine = "Exception Street",
                PinCode = "123456",
                City = "City",
                State = "State",
                AddressType = "Home"
            };

            _mockAddressBL.Setup(bl => bl.AddAddress(input, 1)).ThrowsAsync(new Exception("Database error"));

            var result = await _controller.AddAddress(input) as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
        }
    }
}
