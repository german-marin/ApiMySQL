using ApiMySQL.Controllers;
using ApiMySQL.DTOs;
using ApiMySQL.Model;
using ApiMySQL.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ApiMySQL.Tests.Controllers
{
    [TestFixture]
    public class ExerciseControllerTests
    {
        private Mock<IExerciseRepository> _exerciseRepositoryMock;
        private Mock<ILogger<ExerciseController>> _loggerMock;
        private IMapper _mapper;
        private ExerciseController _controller;

        [SetUp]
        public void Setup()
        {
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();
            _loggerMock = new Mock<ILogger<ExerciseController>>();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Exercise, ExerciseDto>().ReverseMap());
            _mapper = config.CreateMapper();

            _controller = new ExerciseController(_exerciseRepositoryMock.Object, _loggerMock.Object, _mapper);
        }

        [Test]
        public async Task InsertExercise_ValidExercise_ReturnsCreatedResult()
        {
            // Arrange
            var exerciseRepositoryMock = new Mock<IExerciseRepository>();
            var loggerMock = new Mock<ILogger<ExerciseController>>();
            var mapperMock = new Mock<IMapper>();
            var controller = new ExerciseController(exerciseRepositoryMock.Object, loggerMock.Object, mapperMock.Object);

            var exerciseToInsertDto = new ExerciseDto
            {
                Description = "Test Exercise",
                CategoryID = 1,
                Image = "image.jpg"
            };

            var exerciseToInsert = new Exercise
            {
                Description = exerciseToInsertDto.Description,
                CategoryID = exerciseToInsertDto.CategoryID,
                Image = exerciseToInsertDto.Image,
                LastUpdated = DateTime.Now // Simulating server-side timestamp
            };
            // Configurar el mock para el mapeo de MuscleGroupDto a MuscleGroup
            mapperMock.Setup(m => m.Map<Exercise>(It.IsAny<ExerciseDto>())).Returns(exerciseToInsert);

            // Configurar el mock para el repositorio
            exerciseRepositoryMock.Setup(repo => repo.InsertExercise(It.IsAny<Exercise>())).ReturnsAsync(true);

            // Configurar el mock para el mapeo de MuscleGroup a MuscleGroupDto      
            mapperMock.Setup(m => m.Map<ExerciseDto>(exerciseToInsert)).Returns(exerciseToInsertDto);   

            // Act
            var result = await controller.InsertExercise(exerciseToInsertDto) as CreatedResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);

            // Verificar que el método del repositorio fue llamado una vez
            exerciseRepositoryMock.Verify(repo => repo.InsertExercise(It.IsAny<Exercise>()), Times.Once);
        }






        [Test]
        public async Task UpdateExercise_ValidExercise_ReturnsNoContent()
        {
            // Arrange
            var exerciseToUpdate = new ExerciseDto
            {
                ID = 1,
                Description = "Updated Exercise",
                CategoryID = 1,
                Image = "updated.jpg"
            };

            var existingExercise = new Exercise
            {
                ID = 1,
                Description = "Test Exercise",
                CategoryID = 1,
                Image = "image.jpg"
            };

            _exerciseRepositoryMock.Setup(repo => repo.GetExercise(1)).ReturnsAsync(existingExercise);
            _exerciseRepositoryMock.Setup(repo => repo.UpdateExercise(It.IsAny<Exercise>())).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateExercise(exerciseToUpdate) as NoContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);

            _exerciseRepositoryMock.Verify(repo => repo.GetExercise(1), Times.Once);
            _exerciseRepositoryMock.Verify(repo => repo.UpdateExercise(It.IsAny<Exercise>()), Times.Once);
        }

        [Test]
        public async Task GetExercise_ValidId_ReturnsOkResult()
        {
            // Arrange
            var expectedExercise = new Exercise
            {
                ID = 1,
                Description = "Test Exercise",
                CategoryID = 1,
                Image = "image.jpg"
            };

            _exerciseRepositoryMock.Setup(repo => repo.GetExercise(1)).ReturnsAsync(expectedExercise);

            // Act
            var result = await _controller.GetExercise(1) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var actualExerciseDto = result.Value as ExerciseDto;
            Assert.IsNotNull(actualExerciseDto);
            Assert.AreEqual(expectedExercise.ID, actualExerciseDto.ID);
            Assert.AreEqual(expectedExercise.Description, actualExerciseDto.Description);
            Assert.AreEqual(expectedExercise.CategoryID, actualExerciseDto.CategoryID);
            Assert.AreEqual(expectedExercise.Image, actualExerciseDto.Image);
        }

        [Test]
        public async Task GetCategoryExercises_ValidCategoryId_ReturnsOkResult()
        {
            // Arrange
            var expectedExercises = new List<Exercise>
            {
                new Exercise { ID = 1, Description = "Exercise 1", CategoryID = 1, Image = "image1.jpg" },
                new Exercise { ID = 2, Description = "Exercise 2", CategoryID = 1, Image = "image2.jpg" },
            };

            _exerciseRepositoryMock.Setup(repo => repo.GetCategoryExercises(1)).ReturnsAsync(expectedExercises);

            // Act
            var result = await _controller.GetCategoryExercises(1) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var actualExercises = result.Value as List<ExerciseDto>;
            Assert.IsNotNull(actualExercises);
            Assert.AreEqual(expectedExercises.Count, actualExercises.Count);
            Assert.AreEqual(expectedExercises[0].ID, actualExercises[0].ID);
            Assert.AreEqual(expectedExercises[1].ID, actualExercises[1].ID);
            // Check more properties if necessary
        }

        [Test]
        public async Task DeleteExercise_ValidId_ReturnsNoContent()
        {
            // Arrange
            var existingExercise = new Exercise
            {
                ID = 1,
                Description = "Test Exercise",
                CategoryID = 1,
                Image = "image.jpg"
            };

            _exerciseRepositoryMock.Setup(repo => repo.GetExercise(1)).ReturnsAsync(existingExercise);
            _exerciseRepositoryMock.Setup(repo => repo.DeleteExercise(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteExercise(1) as NoContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);

            _exerciseRepositoryMock.Verify(repo => repo.GetExercise(1), Times.Once);
            _exerciseRepositoryMock.Verify(repo => repo.DeleteExercise(1), Times.Once);
        }
    }
}
