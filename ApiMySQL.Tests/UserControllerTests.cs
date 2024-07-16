using ApiMySQL.Controllers;
using ApiMySQL.Model;
using ApiMySQL.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ApiMySQL.DTOs;
using AutoMapper;
using ApiMySQL.Mapping;

namespace ApiMySQL.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> userServiceMock;
        private Mock<ILogger<UserController>> loggerMock;
        private IMapper mapper;

        [SetUp]
        public void Setup()
        {
            userServiceMock = new Mock<IUserService>();
            loggerMock = new Mock<ILogger<UserController>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            mapper = config.CreateMapper();
        }

        [Test]
        public async Task Authenticate_ValidUser_ReturnsOkResultWithToken()
        {
            // Arrange
            var controller = new UserController(userServiceMock.Object, mapper, loggerMock.Object);
            var validUserDTO = new UserDto { Username = "testuser", Password = "testpassword" };
            var validToken = "validToken";

            userServiceMock.Setup(service => service.GetUserByUsernameAndPassword(validUserDTO.Username, validUserDTO.Password))
                           .ReturnsAsync(new User()); // Aquí podrías devolver un User simulado

            userServiceMock.Setup(service => service.GenerateJwtToken(It.IsAny<User>()))
                           .Returns(validToken);

            // Act
            var result = await controller.Authenticate(validUserDTO) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var tokenObject = JObject.FromObject(result.Value);
            Assert.IsNotNull(tokenObject["Token"]); // Verificar que la propiedad "Token" exista

            var tokenValue = tokenObject["Token"].ToString(); // Obtener el valor de la propiedad "Token"
            Assert.AreEqual(validToken, tokenValue); // Comparar el valor del token
        }

        [Test]
        public async Task Authenticate_InvalidUser_ReturnsUnauthorizedResult()
        {
            // Arrange
            var controller = new UserController(userServiceMock.Object, mapper, loggerMock.Object);
            var invalidUserDTO = new UserDto { Username = "invaliduser", Password = "invalidpassword" };

            userServiceMock.Setup(service => service.GetUserByUsernameAndPassword(invalidUserDTO.Username, invalidUserDTO.Password))
                           .ReturnsAsync((User)null);

            // Act
            var result = await controller.Authenticate(invalidUserDTO) as UnauthorizedResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
        }

        [Test]
        public async Task Authenticate_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var controller = new UserController(userServiceMock.Object, mapper, loggerMock.Object);
            var userDTO = new UserDto { Username = "testuser", Password = "testpassword" };

            userServiceMock.Setup(service => service.GetUserByUsernameAndPassword(userDTO.Username, userDTO.Password))
                           .ThrowsAsync(new System.Exception());

            // Act
            var result = await controller.Authenticate(userDTO) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
