using ApiMySQL.Controllers;
using ApiMySQL.Model;
using ApiMySQL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AutoMapper;
using ApiMySQL.DTOs;

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
            var mapperMock = new Mock<IMapper>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object, mapperMock.Object);
            var trainingToInsertDto = new TrainingDto
            {
                Description = "Test Training",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                CustomerID = 1,
                Notes = "Test Notes"
            };
            var trainingToInsert = new Training
            {
                Description = trainingToInsertDto.Description,
                StartDate = trainingToInsertDto.StartDate,
                EndDate = trainingToInsertDto.EndDate,
                CustomerID = trainingToInsertDto.CustomerID,
                Notes = trainingToInsertDto.Notes
            };

            mapperMock.Setup(m => m.Map<Training>(trainingToInsertDto)).Returns(trainingToInsert);

            // Act
            var result = await controller.InsertTraining(trainingToInsertDto) as CreatedResult;

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
            var mapperMock = new Mock<IMapper>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object, mapperMock.Object);
            var trainingToUpdateDto = new TrainingDto
            {
                ID = 1,
                Description = "Updated Training",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                CustomerID = 2,
                Notes = "Updated Notes"
            };
            var trainingToUpdate = new Training
            {
                ID = trainingToUpdateDto.ID,
                Description = trainingToUpdateDto.Description,
                StartDate = trainingToUpdateDto.StartDate,
                EndDate = trainingToUpdateDto.EndDate,
                CustomerID = trainingToUpdateDto.CustomerID,
                Notes = trainingToUpdateDto.Notes
            };

            mapperMock.Setup(m => m.Map<Training>(trainingToUpdateDto)).Returns(trainingToUpdate);

            // Act
            var result = await controller.UpdateTraining(trainingToUpdateDto) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            trainingRepositoryMock.Verify(repo => repo.UpdateTraining(It.IsAny<Training>()), Times.Once);
        }

        [Test]
        public async Task DeleteTraining_ValidId_ReturnsOkResult()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var mapperMock = new Mock<IMapper>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object, mapperMock.Object);
            var trainingIdToDelete = 1;

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
            var mapperMock = new Mock<IMapper>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object, mapperMock.Object);

            var expectedTraining = new Training
            {
                ID = 1,
                Description = "Test Training",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                CustomerID = 1,
                Notes = "Test Notes"
            };

            var expectedTrainingDto = new TrainingDto
            {
                ID = 1,
                Description = "Test Training",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                CustomerID = 1,
                Notes = "Test Notes"
            };

            trainingRepositoryMock.Setup(repo => repo.GetTraining(It.IsAny<int>())).ReturnsAsync(expectedTraining);
            mapperMock.Setup(m => m.Map<TrainingDto>(It.IsAny<Training>())).Returns(expectedTrainingDto);

            // Act
            var result = await controller.GetTraining(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualTraining = result.Value as TrainingDto;
            Assert.That(actualTraining, Is.Not.Null);
            Assert.That(actualTraining, Is.EqualTo(expectedTrainingDto));
        }


        [Test]
        public async Task GetAllTrainings_ReturnsOkResultWithTrainings()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var mapperMock = new Mock<IMapper>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object, mapperMock.Object);

            var expectedTrainings = new List<Training>
            {
                new Training { ID = 1, Description = "Training 1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(7), CustomerID = 1, Notes = "Notes 1" },
                new Training { ID = 2, Description = "Training 2", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(14), CustomerID = 2, Notes = "Notes 2" }
            };

            var expectedTrainingDtos = new List<TrainingDto>
            {
                new TrainingDto { ID = 1, Description = "Training 1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(7), CustomerID = 1, Notes = "Notes 1" },
                new TrainingDto { ID = 2, Description = "Training 2", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(14), CustomerID = 2, Notes = "Notes 2" }
            };

            trainingRepositoryMock.Setup(repo => repo.GetAllTrainings()).ReturnsAsync(expectedTrainings);
            mapperMock.Setup(m => m.Map<IEnumerable<TrainingDto>>(It.IsAny<IEnumerable<Training>>())).Returns(expectedTrainingDtos);

            // Act
            var result = await controller.GetAllTrainings() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualTrainings = result.Value as IEnumerable<TrainingDto>;
            Assert.That(actualTrainings, Is.Not.Null);
            Assert.That(actualTrainings, Is.EquivalentTo(expectedTrainingDtos));
        }


        [Test]
        public async Task GetAllTrainings_ReturnsNoContentWhenNoTrainings()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var mapperMock = new Mock<IMapper>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object, mapperMock.Object);

            // Act
            var result = await controller.GetAllTrainings() as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public async Task DeleteTrainingAndTrainingLines_ValidId_ReturnsOkResult()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var mapperMock = new Mock<IMapper>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object, mapperMock.Object);
            var trainingIdToDelete = 1;

            trainingRepositoryMock.Setup(repo => repo.GetTraining(trainingIdToDelete)).ReturnsAsync(new Training());

            // Act
            var result = await controller.DeleteTrainingAndTrainingLines(trainingIdToDelete);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());

            trainingRepositoryMock.Verify(repo => repo.DeleteTrainingAndTrainingLines(trainingIdToDelete), Times.Once);
        }

        [Test]
        public async Task DeleteTrainingAndTrainingLines_NonExistentTraining_ReturnsNoContent()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var mapperMock = new Mock<IMapper>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object, mapperMock.Object);
            var trainingIdToDelete = 1;

            trainingRepositoryMock.Setup(repo => repo.GetTraining(trainingIdToDelete)).ReturnsAsync((Training)null);

            // Act
            var result = await controller.DeleteTrainingAndTrainingLines(trainingIdToDelete);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());

            trainingRepositoryMock.Verify(repo => repo.DeleteTrainingAndTrainingLines(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task DeleteTrainingAndTrainingLines_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var loggerMock = new Mock<ILogger<TrainingController>>();
            var mapperMock = new Mock<IMapper>();
            var controller = new TrainingController(trainingRepositoryMock.Object, loggerMock.Object, mapperMock.Object);
            var trainingIdToDelete = 1;

            trainingRepositoryMock.Setup(repo => repo.GetTraining(trainingIdToDelete)).ReturnsAsync(new Training());
            trainingRepositoryMock.Setup(repo => repo.DeleteTrainingAndTrainingLines(trainingIdToDelete)).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await controller.DeleteTrainingAndTrainingLines(trainingIdToDelete);

            // Assert
            Assert.That(result, Is.TypeOf<ObjectResult>());

            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.EqualTo("Error interno del servidor."));

            trainingRepositoryMock.Verify(repo => repo.DeleteTrainingAndTrainingLines(trainingIdToDelete), Times.Once);
        }
    }
}
