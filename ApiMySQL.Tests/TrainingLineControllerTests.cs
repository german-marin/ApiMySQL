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
    public class TrainingLineControllerTests
    {
        [Test]
        public async Task InsertTrainingLine_ValidTrainingLine_ReturnsCreatedResult()
        {
            // Arrange
            var trainingLineRepositoryMock = new Mock<ITrainingLineRepository>();
            var loggerMock = new Mock<ILogger<TrainingLineController>>();
            var controller = new TrainingLineController(trainingLineRepositoryMock.Object, loggerMock.Object);
            var trainingLineToInsert = new TrainingLine
            {
                ID = 1,
                ExerciseID = 1,
                TrainingID = 1,
                Sets = "3",
                Repetitions = "10",
                Weight = "50 kg",
                Recovery = "2 min",
                Others = "Test Others",
                Notes = "Test Notes"
            };

            // Act
            var result = await controller.InsertTrainingLine(trainingLineToInsert) as CreatedResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));

            trainingLineRepositoryMock.Verify(repo => repo.InsertTrainingLine(It.IsAny<TrainingLine>()), Times.Once);
        }

        [Test]
        public async Task UpdateTrainingLine_ValidTrainingLine_ReturnsNoContent()
        {
            // Arrange
            var trainingLineRepositoryMock = new Mock<ITrainingLineRepository>();
            var loggerMock = new Mock<ILogger<TrainingLineController>>();
            var controller = new TrainingLineController(trainingLineRepositoryMock.Object, loggerMock.Object);
            var trainingLineToUpdate = new TrainingLine
            {
                ID = 1,
                ExerciseID = 2,
                TrainingID = 1,
                Sets = "4",
                Repetitions = "12",
                Weight = "60 kg",
                Recovery = "3 min",
                Others = "Updated Others",
                Notes = "Updated Notes"
            };
            // Configurar el comportamiento del mock para devolver un Training vacío
            trainingLineRepositoryMock.Setup(repo => repo.GetTrainingLine(trainingLineToUpdate.ID)).ReturnsAsync(new TrainingLine());

            // Act
            var result = await controller.UpdateTrainingLine(trainingLineToUpdate) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            trainingLineRepositoryMock.Verify(repo => repo.UpdateTrainingLine(It.IsAny<TrainingLine>()), Times.Once);
        }

        [Test]
        public async Task DeleteTrainingLine_ValidId_ReturnsNoContent()
        {
            // Arrange
            var trainingLineRepositoryMock = new Mock<ITrainingLineRepository>();
            var loggerMock = new Mock<ILogger<TrainingLineController>>();
            var controller = new TrainingLineController(trainingLineRepositoryMock.Object, loggerMock.Object);
            var trainingLineIdToDelete = 1;

            // Configurar el comportamiento del mock para devolver un Training vacío
            trainingLineRepositoryMock.Setup(repo => repo.GetTrainingLine(trainingLineIdToDelete)).ReturnsAsync(new TrainingLine());

            // Act
            var result = await controller.DeleteTrainingLine(trainingLineIdToDelete);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());

            trainingLineRepositoryMock.Verify(repo => repo.DeleteTrainingLine(trainingLineIdToDelete), Times.Once);
        }

        [Test]
        public async Task GetTrainingLine_ValidId_ReturnsOkResult()
        {
            // Arrange
            var trainingLineRepositoryMock = new Mock<ITrainingLineRepository>();
            var loggerMock = new Mock<ILogger<TrainingLineController>>();
            var controller = new TrainingLineController(trainingLineRepositoryMock.Object, loggerMock.Object);
            var expectedTrainingLine = new TrainingLine
            {
                ID = 1,
                ExerciseID = 1,
                TrainingID = 1,
                Sets = "3",
                Repetitions = "10",
                Weight = "50 kg",
                Recovery = "2 min",
                Others = "Test Others",
                Notes = "Test Notes"
            };

            trainingLineRepositoryMock.Setup(repo => repo.GetTrainingLine(It.IsAny<int>()))
                .ReturnsAsync(expectedTrainingLine);

            // Act
            var result = await controller.GetTrainingLine(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualTrainingLine = result.Value as TrainingLine;
            Assert.That(actualTrainingLine, Is.EqualTo(expectedTrainingLine));
        }

        [Test]
        public async Task GetTrainingLinesOfTraining_ValidId_ReturnsOkResult()
        {
            // Arrange
            var trainingLineRepositoryMock = new Mock<ITrainingLineRepository>();
            var loggerMock = new Mock<ILogger<TrainingLineController>>();
            var controller = new TrainingLineController(trainingLineRepositoryMock.Object, loggerMock.Object);
            var expectedTrainingLines = new List<TrainingLine>
            {
                new TrainingLine { ID = 1, ExerciseID = 1, TrainingID = 1 },
                new TrainingLine { ID = 2, ExerciseID = 2, TrainingID = 1 },
            };

            trainingLineRepositoryMock.Setup(repo => repo.GetTrainingLinesOfTraining(It.IsAny<int>()))
                .ReturnsAsync(expectedTrainingLines);

            // Act
            var result = await controller.GetTrainingLinesOfTraining(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualTrainingLines = result.Value as IEnumerable<TrainingLine>;
            Assert.That(actualTrainingLines, Is.EqualTo(expectedTrainingLines));
        }
    }
}
