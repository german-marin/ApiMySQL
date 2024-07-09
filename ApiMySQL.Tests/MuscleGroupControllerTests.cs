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
    public class MuscleGroupControllerTests
    {
        [Test]
        public async Task InsertMuscleGroup_ValidMuscleGroup_ReturnsCreatedResult()
        {
            // Arrange
            var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();
            var loggerMock = new Mock<ILogger<MuscleGroupController>>();
            var controller = new MuscleGroupController(muscleGroupRepositoryMock.Object, loggerMock.Object);
            var muscleGroupToInsert = new MuscleGroup
            {
                ID = 1,
                Description = "Test Muscle Group",
                ImageFront = "front.jpg",
                ImageRear = "rear.jpg"
            };

            // Act
            var result = await controller.InsertMuscleGroup(muscleGroupToInsert) as CreatedResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));

            muscleGroupRepositoryMock.Verify(repo => repo.InsertMuscleGroup(It.IsAny<MuscleGroup>()), Times.Once);
        }

        [Test]
        public async Task UpdateMuscleGroup_ValidMuscleGroup_ReturnsNoContent()
        {
            // Arrange
            var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();
            var loggerMock = new Mock<ILogger<MuscleGroupController>>();
            var controller = new MuscleGroupController(muscleGroupRepositoryMock.Object, loggerMock.Object);
            var muscleGroupToUpdate = new MuscleGroup
            {
                ID = 1,
                Description = "Updated Muscle Group",
                ImageFront = "updated_front.jpg",
                ImageRear = "updated_rear.jpg"
            };

            // Configurar el comportamiento del mock para devolver un muscleGroup
            muscleGroupRepositoryMock.Setup(repo => repo.GetMuscleGroup(muscleGroupToUpdate.ID)).ReturnsAsync(new MuscleGroup());

            // Act
            var result = await controller.UpdateMuscleGroup(muscleGroupToUpdate) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            muscleGroupRepositoryMock.Verify(repo => repo.UpdateMuscleGroup(It.IsAny<MuscleGroup>()), Times.Once);
        }

        [Test]
        public async Task DeleteMuscleGroup_ValidId_ReturnsNoContent()
        {
            // Arrange
            var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();
            var loggerMock = new Mock<ILogger<MuscleGroupController>>();
            var controller = new MuscleGroupController(muscleGroupRepositoryMock.Object, loggerMock.Object);
            var muscleGroupIdToDelete = 1;

            // Configurar el comportamiento del mock para devolver un muscleGroup
            muscleGroupRepositoryMock.Setup(repo => repo.GetMuscleGroup(muscleGroupIdToDelete)).ReturnsAsync(new MuscleGroup());
            // Act
            var result = await controller.DeleteMuscleGroup(muscleGroupIdToDelete) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            muscleGroupRepositoryMock.Verify(repo => repo.DeleteMuscleGroup(It.IsAny<int>()), Times.Once);            
        }

      


        [Test]
        public async Task GetAllMuscleGroup_ReturnsOkResult()
        {
            // Arrange
            var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();
            var loggerMock = new Mock<ILogger<MuscleGroupController>>();
            var controller = new MuscleGroupController(muscleGroupRepositoryMock.Object, loggerMock.Object);
            var expectedMuscleGroups = new List<MuscleGroup>
            {
                new MuscleGroup { ID = 1, Description = "Muscle Group 1", ImageFront = "front1.jpg", ImageRear = "rear1.jpg" },
                new MuscleGroup { ID = 2, Description = "Muscle Group 2", ImageFront = "front2.jpg", ImageRear = "rear2.jpg" },
            };

            muscleGroupRepositoryMock.Setup(repo => repo.GetAllMuscleGroup())
                .ReturnsAsync(expectedMuscleGroups);

            // Act
            var result = await controller.GetAllMuscleGroup() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualMuscleGroups = result.Value as IEnumerable<MuscleGroup>;
            Assert.That(actualMuscleGroups, Is.EqualTo(expectedMuscleGroups));
        }

        [Test]
        public async Task GetMuscleGroup_ValidId_ReturnsOkResult()
        {
            // Arrange
            var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();
            var loggerMock = new Mock<ILogger<MuscleGroupController>>();
            var controller = new MuscleGroupController(muscleGroupRepositoryMock.Object, loggerMock.Object);
            var expectedMuscleGroup = new MuscleGroup { ID = 1, Description = "Muscle Group 1", ImageFront = "front1.jpg", ImageRear = "rear1.jpg" };

            muscleGroupRepositoryMock.Setup(repo => repo.GetMuscleGroup(It.IsAny<int>()))
                .ReturnsAsync(expectedMuscleGroup);

            // Act
            var result = await controller.GetMuscleGroup(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualMuscleGroup = result.Value as MuscleGroup;
            Assert.That(actualMuscleGroup, Is.EqualTo(expectedMuscleGroup));
        }
    }
}
