using ApiMySQL.Controllers;
using ApiMySQL.Model;
using ApiMySQL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ApiMySQL.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        [Test]
        public async Task Authenticate_ValidUser_ReturnsOkResultWithToken()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<UserController>>();
            var controller = new UserController(userServiceMock.Object, loggerMock.Object);
            var validUser = new User { Username = "testuser", Password = "testpassword" };
            var validToken = "validToken";

            userServiceMock.Setup(service => service.GetUserByUsernameAndPassword(validUser.Username, validUser.Password))
                           .ReturnsAsync(new User());
            userServiceMock.Setup(service => service.GenerateJwtToken(It.IsAny<User>()))
                           .Returns(validToken);

            // Act
            var result = await controller.Authenticate(validUser) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var tokenObject = result.Value as dynamic; // Convertir el resultado a un objeto dinámico
            Assert.IsNotNull(tokenObject); // Verificar que se pudo convertir correctamente

            var tokenValue = tokenObject.Token; // Obtener el valor de la propiedad "Token"
            Assert.AreEqual(validToken, tokenValue); // Comparar el valor del token
        }

        [Test]
        public async Task Authenticate_InvalidUser_ReturnsUnauthorizedResult()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<UserController>>();
            var controller = new UserController(userServiceMock.Object, loggerMock.Object);
            var invalidUser = new User { Username = "invaliduser", Password = "invalidpassword" };

            userServiceMock.Setup(service => service.GetUserByUsernameAndPassword(invalidUser.Username, invalidUser.Password))
               .ReturnsAsync((User)null);


            // Act
            var result = await controller.Authenticate(invalidUser) as UnauthorizedResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
        }

        [Test]
        public async Task Authenticate_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<UserController>>();
            var controller = new UserController(userServiceMock.Object, loggerMock.Object);
            var user = new User { Username = "testuser", Password = "testpassword" };

            userServiceMock.Setup(service => service.GetUserByUsernameAndPassword(user.Username, user.Password))
                           .ThrowsAsync(new System.Exception());

            // Act
            var result = await controller.Authenticate(user) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
