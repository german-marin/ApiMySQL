using ApiMySQL.Controllers;
using ApiMySQL.Model;
using ApiMySQL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Microsoft.Extensions.Logging;

namespace ApiMySQL.Tests.Controllers
{
    [TestFixture]
    public class TrainingControllerTests
    {
        [Test]
        public async Task InsertTraining_ValidTraining_ReturnsCreatedResult()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(repo => repo.CustomerExist(It.IsAny<int>())).ReturnsAsync(true);
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object);
            var trainingToInsert = new Training
            {
                ID = 1,
                Description = "Test Training",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                CustomerID = 1,
                Notes = "Test Notes"
            };

            // Act
            var result = await controller.InsertTraining(trainingToInsert) as CreatedResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));

            trainingRepositoryMock.Verify(repo => repo.InsertTraining(It.IsAny<Training>()), Times.Once);
        }

        [Test]
        public async Task UpdateTraining_ValidTraining_ReturnsNoContent()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(repo => repo.CustomerExist(It.IsAny<int>())).ReturnsAsync(true);
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object);
            var trainingToUpdate = new Training
            {
                ID = 1,
                Description = "Updated Training",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                CustomerID = 2,
                Notes = "Updated Notes"
            };

            // Act
            var result = await controller.UpdateTraining(trainingToUpdate) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            trainingRepositoryMock.Verify(repo => repo.UpdateTraining(It.IsAny<Training>()), Times.Once);
        }

        [Test]
        public async Task DeleteTraining_ValidId_ReturnsNoContent()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object);
            var trainingIdToDelete = 1;

            // Configurar el comportamiento del mock para devolver un Training vacío 
            trainingRepositoryMock.Setup(repo => repo.GetTraining(trainingIdToDelete)).ReturnsAsync(new Training());

            // Act
            var result = await controller.DeleteTraining(trainingIdToDelete);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());

            trainingRepositoryMock.Verify(repo => repo.DeleteTraining(trainingIdToDelete), Times.Once);
        }

        [Test]
        public async Task GetTraining_ValidId_ReturnsOkResult()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object);
            var expectedTraining = new Training
            {
                ID = 1,
                Description = "Test Training",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                CustomerID = 1,
                Notes = "Test Notes"
            };

            trainingRepositoryMock.Setup(repo => repo.GetTraining(It.IsAny<int>()))
                .ReturnsAsync(expectedTraining);

            // Act
            var result = await controller.GetTraining(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualTraining = result.Value as Training;
            Assert.That(actualTraining, Is.EqualTo(expectedTraining));
        }

        [Test]
        public async Task GetAllTrainings_ReturnsOkResultWithTrainings()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object);

            var expectedTrainings = new List<Training>
        {
            new Training { ID = 1, Description = "Training 1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(7), CustomerID = 1, Notes = "Notes 1" },
            new Training { ID = 2, Description = "Training 2", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(14), CustomerID = 2, Notes = "Notes 2" }
            // ... agregar más entrenamientos según sea necesario
        };

            trainingRepositoryMock.Setup(repo => repo.GetAllTrainings())
                .ReturnsAsync(expectedTrainings);

            // Act
            var result = await controller.GetAllTrainings() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualTrainings = result.Value as IEnumerable<Training>;
            Assert.That(actualTrainings, Is.Not.Null);
            Assert.That(actualTrainings, Is.EquivalentTo(expectedTrainings));
        }

        [Test]
        public async Task GetAllTrainings_ReturnsNoContentWhenNoTrainings()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object);

            // No se configura el mock para devolver entrenamientos, simula el escenario de que no hay entrenamientos en la base de datos.

            // Act
            var result = await controller.GetAllTrainings() as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public async Task DeleteTrainingAndTrainingLines_ValidId_ReturnsNoContent()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object);
            var trainingIdToDelete = 1;

            // Configurar el comportamiento del mock para devolver un Training vacío
            trainingRepositoryMock.Setup(repo => repo.GetTraining(trainingIdToDelete)).ReturnsAsync(new Training());

            // Act
            var result = await controller.DeleteTrainingAndTrainingLines(trainingIdToDelete);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());

            // Verificar que el método DeleteTrainingAndTrainingLines se llamó con el id esperado
            trainingRepositoryMock.Verify(repo => repo.DeleteTrainingAndTrainingLines(trainingIdToDelete), Times.Once);
        }

        [Test]
        public async Task DeleteTrainingAndTrainingLines_NonExistentTraining_ReturnsNoContent()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object);
            var trainingIdToDelete = 1;

            // Configurar el comportamiento del mock para devolver null, indicando que el Training no existe
            trainingRepositoryMock.Setup(repo => repo.GetTraining(trainingIdToDelete)).ReturnsAsync((Training)null);

            // Act
            var result = await controller.DeleteTrainingAndTrainingLines(trainingIdToDelete);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());

            // Verificar que el método DeleteTrainingAndTrainingLines no se llamó, ya que no debería existir el entrenamiento
            trainingRepositoryMock.Verify(repo => repo.DeleteTrainingAndTrainingLines(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task DeleteTrainingAndTrainingLines_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object);
            var trainingIdToDelete = 1;

            // Configurar el comportamiento del mock para lanzar una excepción al llamar a DeleteTrainingAndTrainingLines
            trainingRepositoryMock.Setup(repo => repo.GetTraining(trainingIdToDelete)).ReturnsAsync(new Training());
            trainingRepositoryMock.Setup(repo => repo.DeleteTrainingAndTrainingLines(trainingIdToDelete)).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await controller.DeleteTrainingAndTrainingLines(trainingIdToDelete);

            // Assert
            Assert.That(result, Is.TypeOf<ObjectResult>());

            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.EqualTo("Error interno del servidor."));

            // Verificar que se llamó al método DeleteTrainingAndTrainingLines con el id esperado
            trainingRepositoryMock.Verify(repo => repo.DeleteTrainingAndTrainingLines(trainingIdToDelete), Times.Once);
        }
    }
}

