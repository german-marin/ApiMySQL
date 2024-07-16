using ApiMySQL.Controllers;
using ApiMySQL.DTOs;
using ApiMySQL.Model;
using ApiMySQL.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiMySQL.Tests.Controllers
{
    [TestFixture]
    public class CustomerControllerTests
    {
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private Mock<ILogger<CustomerController>> _loggerMock;
        private IMapper _mapper;
        private CustomerController _controller;

        [SetUp]
        public void Setup()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<CustomerController>>();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Customer, CustomerDto>().ReverseMap());
            _mapper = config.CreateMapper();

            _controller = new CustomerController(_customerRepositoryMock.Object, _loggerMock.Object, _mapper);
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
            Assert.IsInstanceOf<List<CustomerDto>>(result.Value);
            var returnCustomers = result.Value as List<CustomerDto>;
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
            Assert.IsInstanceOf<CustomerDto>(result.Value);
            var returnCustomer = result.Value as CustomerDto;
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
            var customerDto = new CustomerDto { FirstName = "John", LastName1 = "Doe" };
            _customerRepositoryMock.Setup(repo => repo.InsertCustomer(It.IsAny<Customer>())).ReturnsAsync(true);

            // Act
            var result = await _controller.InsertCustomer(customerDto) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            Assert.IsInstanceOf<CustomerDto>(result.Value);
        }

        [Test]
        public async Task InsertCustomer_InvalidCustomer_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("FirstName", "Required");

            // Act
            var result = await _controller.InsertCustomer(null);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task UpdateCustomer_ValidCustomer_ReturnsNoContentResult()
        {
            // Arrange
            var customerDto = new CustomerDto { ID = 1, FirstName = "John", LastName1 = "Doe" };
            var customer = new Customer { ID = 1, FirstName = "John", LastName1 = "Doe" };
            _customerRepositoryMock.Setup(repo => repo.GetCustomer(1)).ReturnsAsync(customer);
            _customerRepositoryMock.Setup(repo => repo.UpdateCustomer(It.IsAny<Customer>())).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateCustomer(customerDto);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task UpdateCustomer_InvalidCustomer_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("FirstName", "Required");

            // Act
            var result = await _controller.UpdateCustomer(null);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
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