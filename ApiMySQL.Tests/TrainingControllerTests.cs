using ApiMySQL.Controllers;
using ApiMySQL.Model;
using ApiMySQL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

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
            var controller = new TrainingController(trainingRepositoryMock.Object);
            var trainingToInsert = new Training
            {
                ID = 1,
                Description = "Test Training",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                IdClient = 1,
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
            var controller = new TrainingController(trainingRepositoryMock.Object);
            var trainingToUpdate = new Training
            {
                ID = 1,
                Description = "Updated Training",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                IdClient = 2,
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
            var controller = new TrainingController(trainingRepositoryMock.Object);
            var trainingIdToDelete = 1;

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
            var controller = new TrainingController(trainingRepositoryMock.Object);
            var expectedTraining = new Training
            {
                ID = 1,
                Description = "Test Training",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                IdClient = 1,
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
                var controller = new TrainingController(trainingRepositoryMock.Object);

                var expectedTrainings = new List<Training>
        {
            new Training { ID = 1, Description = "Training 1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(7), IdClient = 1, Notes = "Notes 1" },
            new Training { ID = 2, Description = "Training 2", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(14), IdClient = 2, Notes = "Notes 2" }
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
                var controller = new TrainingController(trainingRepositoryMock.Object);

                // No se configura el mock para devolver entrenamientos, simula el escenario de que no hay entrenamientos en la base de datos.

                // Act
                var result = await controller.GetAllTrainings() as NoContentResult;

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.StatusCode, Is.EqualTo(204));
            }
        }

    }

