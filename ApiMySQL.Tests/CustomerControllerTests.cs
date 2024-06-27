using ApiMySQL.Controllers;
using ApiMySQL.Model;
using ApiMySQL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace ApiMySQL.Tests.Controllers
{
    [TestFixture]
    public class CustomerControllerTests
    {
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private Mock<ILogger<CustomerController>> _loggerMock;
        private CustomerController _controller;

        [SetUp]
        public void Setup()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<CustomerController>>();
            _controller = new CustomerController(_customerRepositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAllCustomers_ReturnsOkResult_WithListOfCustomers()
        {
            // Arrange
            var customers = new List<Customer> { new Customer { ID = 1, FirstName = "John", LastName1 = "Doe" } };
            _customerRepositoryMock.Setup(repo => repo.GetAllCustomers()).ReturnsAsync(customers);

            // Act
            var result = await _controller.GetAllCustomers() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOf<List<Customer>>(result.Value);
            var returnCustomers = result.Value as List<Customer>;
            Assert.AreEqual(1, returnCustomers.Count);
        }

        [Test]
        public async Task GetCustomer_ValidId_ReturnsOkResult_WithCustomer()
        {
            // Arrange
            var customer = new Customer { ID = 1, FirstName = "John", LastName1 = "Doe" };
            _customerRepositoryMock.Setup(repo => repo.GetCustomer(1)).ReturnsAsync(customer);

            // Act
            var result = await _controller.GetCustomer(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOf<Customer>(result.Value);
            var returnCustomer = result.Value as Customer;
            Assert.AreEqual(1, returnCustomer.ID);
        }

        [Test]
        public async Task GetCustomer_InvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            _customerRepositoryMock.Setup(repo => repo.GetCustomer(1)).ReturnsAsync((Customer)null);

            // Act
            var result = await _controller.GetCustomer(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task InsertCustomer_ValidCustomer_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var customer = new Customer { ID = 1, FirstName = "John", LastName1 = "Doe" };
            _customerRepositoryMock.Setup(repo => repo.InsertCustomer(customer)).ReturnsAsync(true);

            // Act
            var result = await _controller.InsertCustomer(customer) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            Assert.IsInstanceOf<Customer>(result.Value);
            var returnCustomer = result.Value as Customer;
            Assert.AreEqual(1, returnCustomer.ID);
        }

        [Test]
        public async Task InsertCustomer_NullCustomer_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.InsertCustomer(null) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task UpdateCustomer_ValidCustomer_ReturnsNoContentResult()
        {
            // Arrange
            var customer = new Customer { ID = 1, FirstName = "John", LastName1 = "Doe" };
            _customerRepositoryMock.Setup(repo => repo.GetCustomer(1)).ReturnsAsync(customer);
            _customerRepositoryMock.Setup(repo => repo.UpdateCustomer(customer)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateCustomer(customer);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task UpdateCustomer_NullCustomer_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UpdateCustomer(null) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task DeleteCustomer_ValidId_ReturnsNoContentResult()
        {
            // Arrange
            var customer = new Customer { ID = 1, FirstName = "John", LastName1 = "Doe" };
            _customerRepositoryMock.Setup(repo => repo.GetCustomer(1)).ReturnsAsync(customer);
            _customerRepositoryMock.Setup(repo => repo.DeleteCustomer(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCustomer(1);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteCustomer_InvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            _customerRepositoryMock.Setup(repo => repo.GetCustomer(1)).ReturnsAsync((Customer)null);

            // Act
            var result = await _controller.DeleteCustomer(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
