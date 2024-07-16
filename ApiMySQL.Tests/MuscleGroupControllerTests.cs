using ApiMySQL.Controllers;
using ApiMySQL.DTOs;
using ApiMySQL.Mapping;
using ApiMySQL.Model;
using ApiMySQL.Repositories;
using AutoMapper;
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
    public class MuscleGroupControllerTests
    {
        private Mock<IMuscleGroupRepository> muscleGroupRepositoryMock;
        private Mock<ILogger<MuscleGroupController>> loggerMock;
        private Mock<IMapper> mapperMock;
        private MuscleGroupController controller;

        [SetUp]
        public void SetUp()
        {
            muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();
            loggerMock = new Mock<ILogger<MuscleGroupController>>();
            mapperMock = new Mock<IMapper>();
            controller = new MuscleGroupController(muscleGroupRepositoryMock.Object, loggerMock.Object, mapperMock.Object);
        }

        [Test]
        public async Task InsertMuscleGroup_ValidMuscleGroup_ReturnsCreatedResult()
        {
            // Arrange
            var muscleGroupToInsert = new MuscleGroupDto
            {
                Description = "Test Muscle Group",
                ImageFront = "front.jpg",
                ImageRear = "rear.jpg"
            };

            var insertedMuscleGroup = new MuscleGroup
            {
                ID = 1,
                Description = "Test Muscle Group",
                ImageFront = "front.jpg",
                ImageRear = "rear.jpg"
            };

            // Configurar el mock para el mapeo de MuscleGroupDto a MuscleGroup
            mapperMock.Setup(m => m.Map<MuscleGroup>(It.IsAny<MuscleGroupDto>())).Returns(insertedMuscleGroup);

            // Configurar el mock para el repositorio
            muscleGroupRepositoryMock.Setup(repo => repo.InsertMuscleGroup(It.IsAny<MuscleGroup>())).ReturnsAsync(true);

            // Configurar el mock para el mapeo de MuscleGroup a MuscleGroupDto
            mapperMock.Setup(m => m.Map<MuscleGroupDto>(insertedMuscleGroup)).Returns(muscleGroupToInsert);

            // Act
            var result = await controller.InsertMuscleGroup(muscleGroupToInsert) as CreatedResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));          

            // Verificar que el método del repositorio fue llamado una vez
            muscleGroupRepositoryMock.Verify(repo => repo.InsertMuscleGroup(It.IsAny<MuscleGroup>()), Times.Once);
        }


        [Test]
        public async Task UpdateMuscleGroup_ValidMuscleGroup_ReturnsNoContent()
        {
            // Arrange
            var muscleGroupToUpdate = new MuscleGroupDto
            {
                ID = 1,
                Description = "Updated Muscle Group",
                ImageFront = "updated_front.jpg",
                ImageRear = "updated_rear.jpg"
            };

            var existingMuscleGroup = new MuscleGroup
            {
                ID = 1,
                Description = "Old Muscle Group",
                ImageFront = "old_front.jpg",
                ImageRear = "old_rear.jpg"
            };

            muscleGroupRepositoryMock.Setup(repo => repo.GetMuscleGroup(muscleGroupToUpdate.ID)).ReturnsAsync(existingMuscleGroup);
            mapperMock.Setup(m => m.Map(muscleGroupToUpdate, existingMuscleGroup)).Returns(existingMuscleGroup);
            muscleGroupRepositoryMock.Setup(repo => repo.UpdateMuscleGroup(It.IsAny<MuscleGroup>())).ReturnsAsync(true);

            // Act
            var result = await controller.UpdateMuscleGroup(muscleGroupToUpdate) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            muscleGroupRepositoryMock.Verify(repo => repo.UpdateMuscleGroup(It.IsAny<MuscleGroup>()), Times.Once);
        }

        [Test]
        public async Task DeleteMuscleGroup_ValidId_ReturnsOk()
        {
            // Arrange
            var muscleGroupIdToDelete = 1;
            var existingMuscleGroup = new MuscleGroup
            {
                ID = muscleGroupIdToDelete,
                Description = "Muscle Group to Delete",
                ImageFront = "front.jpg",
                ImageRear = "rear.jpg"
            };

            muscleGroupRepositoryMock.Setup(repo => repo.GetMuscleGroup(muscleGroupIdToDelete)).ReturnsAsync(existingMuscleGroup);
            muscleGroupRepositoryMock.Setup(repo => repo.DeleteMuscleGroup(muscleGroupIdToDelete)).ReturnsAsync(true);

            // Act
            var result = await controller.DeleteMuscleGroup(muscleGroupIdToDelete) as OkResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            muscleGroupRepositoryMock.Verify(repo => repo.DeleteMuscleGroup(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task GetAllMuscleGroup_ReturnsOkResult()
        {
            // Arrange
            var expectedMuscleGroups = new List<MuscleGroup>
            {
                new MuscleGroup { ID = 1, Description = "Muscle Group 1", ImageFront = "front1.jpg", ImageRear = "rear1.jpg" },
                new MuscleGroup { ID = 2, Description = "Muscle Group 2", ImageFront = "front2.jpg", ImageRear = "rear2.jpg" },
            };

            var expectedMuscleGroupDtos = new List<MuscleGroupDto>
            {
                new MuscleGroupDto { ID = 1, Description = "Muscle Group 1", ImageFront = "front1.jpg", ImageRear = "rear1.jpg" },
                new MuscleGroupDto { ID = 2, Description = "Muscle Group 2", ImageFront = "front2.jpg", ImageRear = "rear2.jpg" },
            };

            muscleGroupRepositoryMock.Setup(repo => repo.GetAllMuscleGroup()).ReturnsAsync(expectedMuscleGroups);
            mapperMock.Setup(m => m.Map<IEnumerable<MuscleGroupDto>>(expectedMuscleGroups)).Returns(expectedMuscleGroupDtos);

            // Act
            var result = await controller.GetAllMuscleGroup() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualMuscleGroups = result.Value as IEnumerable<MuscleGroupDto>;
            Assert.That(actualMuscleGroups, Is.EqualTo(expectedMuscleGroupDtos));
        }

        [Test]
        public async Task GetMuscleGroup_ValidId_ReturnsOkResult()
        {
            // Arrange
            var expectedMuscleGroup = new MuscleGroup { ID = 1, Description = "Muscle Group 1", ImageFront = "front1.jpg", ImageRear = "rear1.jpg" };
            var expectedMuscleGroupDto = new MuscleGroupDto { ID = 1, Description = "Muscle Group 1", ImageFront = "front1.jpg", ImageRear = "rear1.jpg" };

            muscleGroupRepositoryMock.Setup(repo => repo.GetMuscleGroup(It.IsAny<int>())).ReturnsAsync(expectedMuscleGroup);
            mapperMock.Setup(m => m.Map<MuscleGroupDto>(expectedMuscleGroup)).Returns(expectedMuscleGroupDto);

            // Act
            var result = await controller.GetMuscleGroup(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualMuscleGroup = result.Value as MuscleGroupDto;
            Assert.That(actualMuscleGroup, Is.EqualTo(expectedMuscleGroupDto));
        }

        [Test]
        public void Map_MuscleGroup_To_MuscleGroupDto()
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));

            var muscleGroup = new MuscleGroup
            {
                ID = 1,
                Description = "Test Muscle Group",
                ImageFront = "front.jpg",
                ImageRear = "rear.jpg",
                LastUpdate = DateTime.Now
            };

            var muscleGroupDto = mapper.Map<MuscleGroupDto>(muscleGroup);

            Assert.AreEqual(muscleGroup.ID, muscleGroupDto.ID);
            Assert.AreEqual(muscleGroup.Description, muscleGroupDto.Description);
            Assert.AreEqual(muscleGroup.ImageFront, muscleGroupDto.ImageFront);
            Assert.AreEqual(muscleGroup.ImageRear, muscleGroupDto.ImageRear);
        }

        [Test]
        public void Map_MuscleGroupDto_To_MuscleGroup()
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));

            var muscleGroupDto = new MuscleGroupDto
            {
                ID = 1,
                Description = "Test Muscle Group",
                ImageFront = "front.jpg",
                ImageRear = "rear.jpg"
            };

            var muscleGroup = mapper.Map<MuscleGroup>(muscleGroupDto);

            Assert.AreEqual(muscleGroupDto.ID, muscleGroup.ID);
            Assert.AreEqual(muscleGroupDto.Description, muscleGroup.Description);
            Assert.AreEqual(muscleGroupDto.ImageFront, muscleGroup.ImageFront);
            Assert.AreEqual(muscleGroupDto.ImageRear, muscleGroup.ImageRear);
            Assert.AreEqual(default(DateTime), muscleGroup.LastUpdate); // La propiedad LastUpdate debería ser su valor predeterminado
        }

    }
}
