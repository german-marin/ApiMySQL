using ApiMySQL.Controllers;
using ApiMySQL.DTOs;
using ApiMySQL.Model;
using ApiMySQL.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiMySQL.Tests.Controllers
{
    [TestFixture]
    public class TrainingLineControllerTests
    {
        private Mock<ITrainingLineRepository> _mockRepo;
        private Mock<ILogger<TrainingLineController>> _mockLogger;
        private Mock<IMapper> _mockMapper;
        private TrainingLineController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<ITrainingLineRepository>();
            _mockLogger = new Mock<ILogger<TrainingLineController>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new TrainingLineController(_mockRepo.Object, _mockLogger.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetTrainingLinesOfTraining_ReturnsOkResult_WithTrainingLines()
        {
            // Arrange
            var trainingLines = new List<TrainingLine>
            {
                new TrainingLine { ID = 1, ExerciseID = 1, TrainingID = 1 },
                new TrainingLine { ID = 2, ExerciseID = 2, TrainingID = 1 }
            };
            var TrainingLineDtos = new List<TrainingLineDto>
            {
                new TrainingLineDto { ID = 1, ExerciseID = 1, TrainingID = 1 },
                new TrainingLineDto { ID = 2, ExerciseID = 2, TrainingID = 1 }
            };

            _mockRepo.Setup(repo => repo.GetTrainingLinesOfTraining(1)).ReturnsAsync(trainingLines);
            _mockMapper.Setup(m => m.Map<IEnumerable<TrainingLineDto>>(trainingLines)).Returns(TrainingLineDtos);

            // Act
            var result = await _controller.GetTrainingLinesOfTraining(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsInstanceOf<IEnumerable<TrainingLineDto>>(okResult.Value);
            var returnValue = okResult.Value as IEnumerable<TrainingLineDto>;
            Assert.AreEqual(2, returnValue.Count());
        }

        [Test]
        public async Task GetTrainingLine_ReturnsOkResult_WithTrainingLine()
        {
            // Arrange
            var trainingLine = new TrainingLine { ID = 1, ExerciseID = 1, TrainingID = 1 };
            var TrainingLineDto = new TrainingLineDto { ID = 1, ExerciseID = 1, TrainingID = 1 };

            _mockRepo.Setup(repo => repo.GetTrainingLine(1)).ReturnsAsync(trainingLine);
            _mockMapper.Setup(m => m.Map<TrainingLineDto>(trainingLine)).Returns(TrainingLineDto);

            // Act
            var result = await _controller.GetTrainingLine(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsInstanceOf<TrainingLineDto>(okResult.Value);
            var returnValue = okResult.Value as TrainingLineDto;
            Assert.AreEqual(1, returnValue.ID);
        }

        [Test]
        public async Task InsertTrainingLine_ReturnsOkResult()
        {
            // Arrange
            var TrainingLineDto = new TrainingLineDto { ID = 1, ExerciseID = 1, TrainingID = 1 };
            var trainingLine = new TrainingLine { ID = 1, ExerciseID = 1, TrainingID = 1 };

            _mockMapper.Setup(m => m.Map<TrainingLine>(TrainingLineDto)).Returns(trainingLine);
            _mockRepo.Setup(repo => repo.InsertTrainingLine(trainingLine)).ReturnsAsync(true);

            // Act
            var result = await _controller.InsertTrainingLine(TrainingLineDto);

            // Assert
            var okResult = result as CreatedResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(201, okResult.StatusCode);
        }

        [Test]
        public async Task UpdateTrainingLine_ReturnsOkResult()
        {
            // Arrange
            var TrainingLineDto = new TrainingLineDto { ID = 1, ExerciseID = 1, TrainingID = 1 };
            var trainingLine = new TrainingLine { ID = 1, ExerciseID = 1, TrainingID = 1 };

            _mockMapper.Setup(m => m.Map<TrainingLine>(TrainingLineDto)).Returns(trainingLine);
            _mockRepo.Setup(repo => repo.UpdateTrainingLine(trainingLine)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateTrainingLine(TrainingLineDto);

            // Assert
            var okResult = result as OkResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task DeleteTrainingLine_ReturnsOkResult()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.DeleteTrainingLine(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTrainingLine(1) as OkObjectResult;

            // Assert          
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }
    }
}
