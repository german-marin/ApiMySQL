using ApiMySQL.Controllers;
using ApiMySQL.Model;
using ApiMySQL.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ApiMySQL.DTOs;

namespace ApiMySQL.Tests.Controllers
{
    [TestFixture]
    public class CategoryControllerTests
    {
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryDto>();
                cfg.CreateMap<CategoryDto, Category>();
            });

            _mapper = configurationProvider.CreateMapper();
        }

        [Test]
        public async Task InsertCategory_ValidCategory_ReturnsCreatedResult()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerMock = new Mock<ILogger<CategoryController>>();
            var controller = new CategoryController(categoryRepositoryMock.Object, loggerMock.Object, _mapper);
            var categoryToInsert = new CategoryDto
            {
                ID = 1,
                Description = "Test Category",
                MuscleGroupID = 1
            };

            categoryRepositoryMock.Setup(repo => repo.InsertCategory(It.IsAny<Category>()))
                .ReturnsAsync(true);

            // Act
            var result = await controller.InsertCategory(categoryToInsert) as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));
            Assert.That(result.ActionName, Is.EqualTo(nameof(controller.GetCategory)));
            Assert.That(result.RouteValues["id"], Is.EqualTo(categoryToInsert.ID));
            Assert.That(result.Value, Is.InstanceOf<CategoryDto>());

            categoryRepositoryMock.Verify(repo => repo.InsertCategory(It.IsAny<Category>()), Times.Once);
        }

        [Test]
        public async Task UpdateCategory_ValidCategory_ReturnsNoContent()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerMock = new Mock<ILogger<CategoryController>>();
            var controller = new CategoryController(categoryRepositoryMock.Object, loggerMock.Object, _mapper);
            var categoryToUpdate = new CategoryDto { ID = 1, Description = "Updated Category", MuscleGroupID = 1 };

            // Configurar el comportamiento del mock para devolver un Training vacío
            categoryRepositoryMock.Setup(repo => repo.GetCategory(categoryToUpdate.ID)).ReturnsAsync(new Category());

            // Act
            var result = await controller.UpdateCategory(categoryToUpdate);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());

            var noContentResult = result as NoContentResult;
            Assert.That(noContentResult.StatusCode, Is.EqualTo(204));

            categoryRepositoryMock.Verify(repo => repo.UpdateCategory(It.IsAny<Category>()), Times.Once);

        }

        [Test]
        public async Task DeleteCategory_ValidId_ReturnsNoContent()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerMock = new Mock<ILogger<CategoryController>>();
            var controller = new CategoryController(categoryRepositoryMock.Object, loggerMock.Object, _mapper);
            var categoryIdToDelete = 1;

            // Configurar el comportamiento del mock para devolver un Training vacío
            categoryRepositoryMock.Setup(repo => repo.GetCategory(categoryIdToDelete)).ReturnsAsync(new Category());

            // Act
            var result = await controller.DeleteCategory(categoryIdToDelete); //as NoContentResult;

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());

            var noContentResult = result as NoContentResult;
            Assert.That(noContentResult.StatusCode, Is.EqualTo(204));
            Assert.That(result, Is.Not.Null);


            categoryRepositoryMock.Verify(repo => repo.DeleteCategory(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task GetMuscleGroupCategories_ValidId_ReturnsOkResult()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerMock = new Mock<ILogger<CategoryController>>();
            var controller = new CategoryController(categoryRepositoryMock.Object, loggerMock.Object, _mapper);
            var expectedCategories = new List<Category>
    {
        new Category { ID = 1, Description = "Category 1", MuscleGroupID = 1 },
        new Category { ID = 2, Description = "Category 2", MuscleGroupID = 1 },
    };

            categoryRepositoryMock.Setup(repo => repo.GetMuscleGroupCategories(It.IsAny<int>()))
                .ReturnsAsync(expectedCategories);

            // Act
            var result = await controller.GetMuscleGroupCategories(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualCategories = result.Value as IEnumerable<CategoryDto>;
            Assert.That(actualCategories, Is.Not.Null);

            var expectedCategoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(expectedCategories);

            // Compare individual properties of each item
            Assert.That(actualCategories.Count(), Is.EqualTo(expectedCategoryDtos.Count()));

            for (int i = 0; i < expectedCategoryDtos.Count(); i++)
            {
                var expectedCategoryDto = expectedCategoryDtos.ElementAt(i);
                var actualCategoryDto = actualCategories.ElementAt(i);

                Assert.That(actualCategoryDto.ID, Is.EqualTo(expectedCategoryDto.ID));
                Assert.That(actualCategoryDto.Description, Is.EqualTo(expectedCategoryDto.Description));
                Assert.That(actualCategoryDto.MuscleGroupID, Is.EqualTo(expectedCategoryDto.MuscleGroupID));
            }

            categoryRepositoryMock.Verify(repo => repo.GetMuscleGroupCategories(It.IsAny<int>()), Times.Once);
        }


        [Test]
        public async Task GetCategory_ValidId_ReturnsOkResult()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerMock = new Mock<ILogger<CategoryController>>();
            var controller = new CategoryController(categoryRepositoryMock.Object, loggerMock.Object, _mapper);
            var expectedCategory = new Category { ID = 1, Description = "Category 1", MuscleGroupID = 1 };

            categoryRepositoryMock.Setup(repo => repo.GetCategory(It.IsAny<int>()))
                .ReturnsAsync(expectedCategory);

            // Act
            var result = await controller.GetCategory(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var actualCategory = _mapper.Map<Category>(result.Value);
            Assert.NotNull(actualCategory);
            Assert.AreEqual(actualCategory.ID, expectedCategory.ID);
            Assert.AreEqual(actualCategory.Description, expectedCategory.Description);
            Assert.AreEqual(actualCategory.MuscleGroupID, expectedCategory.MuscleGroupID);
    
        }

        [Test]
        public async Task GetCategory_ValidId_ReturnsCategory()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerMock = new Mock<ILogger<CategoryController>>();
            var controller = new CategoryController(categoryRepositoryMock.Object, loggerMock.Object, _mapper);
            var expectedCategory = new Category { ID = 1, Description = "Test Category", MuscleGroupID = 1 };

            categoryRepositoryMock.Setup(repo => repo.GetCategory(It.IsAny<int>()))
                .ReturnsAsync(expectedCategory);

            // Act
            var result = await controller.GetCategory(1) as OkObjectResult;
            var expectedCategoryDto = _mapper.Map<CategoryDto>(expectedCategory);
           
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var actualCategoryDto = result.Value as CategoryDto;
            Assert.AreEqual(expectedCategoryDto.ID, actualCategoryDto.ID);
            Assert.AreEqual(expectedCategoryDto.Description, actualCategoryDto.Description);
            Assert.AreEqual(expectedCategoryDto.MuscleGroupID, actualCategoryDto.MuscleGroupID);
            categoryRepositoryMock.Verify(repo => repo.GetCategory(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task InsertCategory_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerMock = new Mock<ILogger<CategoryController>>();
            var controller = new CategoryController(categoryRepositoryMock.Object, loggerMock.Object, _mapper);

            var categoryToInsert = new CategoryDto
            {
                Description = "Test",  // o string.Empty
            };

            // Configurar el comportamiento del mock para devolver un Training vacío
            categoryRepositoryMock.Setup(repo => repo.GetCategory(categoryToInsert.ID)).ReturnsAsync(new Category());

            // Act
            var result = await controller.InsertCategory(categoryToInsert) as BadRequestResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            categoryRepositoryMock.Verify(repo => repo.InsertCategory(It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public async Task UpdateCategory_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerMock = new Mock<ILogger<CategoryController>>();
            var controller = new CategoryController(categoryRepositoryMock.Object, loggerMock.Object, _mapper);

            var categoryToUpdate = new CategoryDto
            {
                Description = "Test",
            };

            // Configurar el comportamiento del mock para devolver un Training vacío
            categoryRepositoryMock.Setup(repo => repo.GetCategory(categoryToUpdate.ID)).ReturnsAsync((Category)null);

            // Act
            var result = await controller.UpdateCategory(categoryToUpdate) as BadRequestResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            categoryRepositoryMock.Verify(repo => repo.UpdateCategory(It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public async Task DeleteCategory_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerMock = new Mock<ILogger<CategoryController>>();
            var controller = new CategoryController(categoryRepositoryMock.Object, loggerMock.Object, _mapper);
            var categoryIdToDelete = 1;

            // Configurar el comportamiento del mock para devolver un Training vacío
            categoryRepositoryMock.Setup(repo => repo.GetCategory(categoryIdToDelete)).ReturnsAsync((Category)null);

            // Act
            var result = await controller.DeleteCategory(categoryIdToDelete) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));
            categoryRepositoryMock.Verify(repo => repo.DeleteCategory(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetCategory_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerMock = new Mock<ILogger<CategoryController>>();
            var controller = new CategoryController(categoryRepositoryMock.Object, loggerMock.Object, _mapper);
            var categoryIdToGet = 1;

            // Configurar el comportamiento del mock para devolver un Training vacío
            categoryRepositoryMock.Setup(repo => repo.GetCategory(categoryIdToGet)).ReturnsAsync((Category)null);

            // Act
            var result = await controller.GetCategory(categoryIdToGet) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));
            categoryRepositoryMock.Verify(repo => repo.GetCategory(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task InsertCategory_NullData_ReturnsBadRequest()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerMock = new Mock<ILogger<CategoryController>>();
            var controller = new CategoryController(categoryRepositoryMock.Object, loggerMock.Object, _mapper);

            // Act
            var result = await controller.InsertCategory(null) as BadRequestResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            categoryRepositoryMock.Verify(repo => repo.InsertCategory(It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public async Task UpdateCategory_NullData_ReturnsBadRequest()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerMock = new Mock<ILogger<CategoryController>>();
            var controller = new CategoryController(categoryRepositoryMock.Object, loggerMock.Object, _mapper);

            // Act
            var result = await controller.UpdateCategory(null) as BadRequestResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            categoryRepositoryMock.Verify(repo => repo.UpdateCategory(It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public async Task UpdateCategory_InvalidId_ReturnsBadRequest()
        {

            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerMock = new Mock<ILogger<CategoryController>>();
            var controller = new CategoryController(categoryRepositoryMock.Object, loggerMock.Object, _mapper);

            var categoryToUpdate = new CategoryDto
            {
                ID = 0,
                Description = "Test",
            };

            // Configurar el comportamiento del mock para devolver un Training vacío
            categoryRepositoryMock.Setup(repo => repo.GetCategory(categoryToUpdate.ID)).ReturnsAsync((Category)null);

            // Act
            var result = await controller.UpdateCategory(categoryToUpdate) as BadRequestResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            categoryRepositoryMock.Verify(repo => repo.UpdateCategory(It.IsAny<Category>()), Times.Never);
        }

    }
}
