using ApiMySQL.Controllers;
using ApiMySQL.Model;
using ApiMySQL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;
namespace ApiMySQL.Tests.Controllers
{
    [TestFixture]
    public class ExerciseControllerTests
    {
        [Test]
        public async Task InsertExercise_ValidExercise_ReturnsCreatedResult()
        {
            // Arrange
            var exerciseRepositoryMock = new Mock<IExerciseRepository>();
            var loggerMock = new Mock<ILogger<ExerciseController>>();
            var controller = new ExerciseController(exerciseRepositoryMock.Object, loggerMock.Object);
            var exerciseToInsert = new Exercise
            {
                ID = 1,
                Description = "Test Exercise",
                CategoryID = 1,
                Image = "image.jpg"
            };

            // Act
            var result = await controller.InsertExercise(exerciseToInsert) as CreatedResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));

            exerciseRepositoryMock.Verify(repo => repo.InsertExercise(It.IsAny<Exercise>()), Times.Once);
        }

        [Test]
        public async Task UpdateExercise_ValidExercise_ReturnsNoContent()
        {
            // Arrange
            var exerciseRepositoryMock = new Mock<IExerciseRepository>();
            var loggerMock = new Mock<ILogger<ExerciseController>>();
            var controller = new ExerciseController(exerciseRepositoryMock.Object, loggerMock.Object);
            var exerciseToUpdate = new Exercise { ID = 1, Description = "Updated Exercise", CategoryID = 1, Image = "updated.jpg" };

            var existingExercise = new Exercise { ID = 1, Description = "Test Exercise", CategoryID = 1, Image = "image.jpg" };
            exerciseRepositoryMock.Setup(repo => repo.GetExercise(It.IsAny<int>()))
                .ReturnsAsync(existingExercise);

            // Act
            var result = await controller.UpdateExercise(exerciseToUpdate) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            exerciseRepositoryMock.Verify(repo => repo.GetExercise(It.IsAny<int>()), Times.Once);
            exerciseRepositoryMock.Verify(repo => repo.UpdateExercise(It.IsAny<Exercise>()), Times.Once);
        }

        [Test]
        public async Task GetExercise_ValidId_ReturnsOkResult()
        {
            // Arrange
            var exerciseRepositoryMock = new Mock<IExerciseRepository>();
            var loggerMock = new Mock<ILogger<ExerciseController>>();
            var controller = new ExerciseController(exerciseRepositoryMock.Object, loggerMock.Object);
            var expectedExercise = new Exercise { ID = 1, Description = "Test Exercise", CategoryID = 1, Image = "image.jpg" };

            exerciseRepositoryMock.Setup(repo => repo.GetExercise(It.IsAny<int>()))
                .ReturnsAsync(expectedExercise);

            // Act
            var result = await controller.GetExercise(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualExercise = result.Value as Exercise;
            Assert.That(actualExercise, Is.EqualTo(expectedExercise));
        }

        [Test]
        public async Task GetCategoryExercises_ValidCategoryId_ReturnsOkResult()
        {
            // Arrange
            var exerciseRepositoryMock = new Mock<IExerciseRepository>();
            var loggerMock = new Mock<ILogger<ExerciseController>>();
            var controller = new ExerciseController(exerciseRepositoryMock.Object, loggerMock.Object);
            var expectedExercises = new List<Exercise>
            {
                new Exercise { ID = 1, Description = "Exercise 1", CategoryID = 1, Image = "image1.jpg" },
                new Exercise { ID = 2, Description = "Exercise 2", CategoryID = 1, Image = "image2.jpg" },
            };

            exerciseRepositoryMock.Setup(repo => repo.GetCategoryExercises(It.IsAny<int>()))
                .ReturnsAsync(expectedExercises);

            // Act
            var result = await controller.GetCategoryExercises(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualExercises = result.Value as IEnumerable<Exercise>;
            Assert.That(actualExercises, Is.EqualTo(expectedExercises));
        }

        [Test]
        public async Task DeleteExercise_ValidId_ReturnsNoContent()
        {
            // Arrange
            var exerciseRepositoryMock = new Mock<IExerciseRepository>();
            var loggerMock = new Mock<ILogger<ExerciseController>>();
            var controller = new ExerciseController(exerciseRepositoryMock.Object, loggerMock.Object);
            var exerciseIdToDelete = 1;

            var existingExercise = new Exercise { ID = 1, Description = "Test Exercise", CategoryID = 1, Image = "image.jpg" };
            exerciseRepositoryMock.Setup(repo => repo.GetExercise(It.IsAny<int>()))
                .ReturnsAsync(existingExercise);

            // Act
            var result = await controller.DeleteExercise(exerciseIdToDelete) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            exerciseRepositoryMock.Verify(repo => repo.GetExercise(It.IsAny<int>()), Times.Once);
            exerciseRepositoryMock.Verify(repo => repo.DeleteExercise(It.IsAny<int>()), Times.Once);
        }
    }
}
