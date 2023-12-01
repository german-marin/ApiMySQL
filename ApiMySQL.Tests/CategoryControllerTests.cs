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
    public class CategoryControllerTests
    {
        [Test]
        public async Task InsertCategory_ValidCategory_ReturnsCreatedResult()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var controller = new CategoryController(categoryRepositoryMock.Object);
            var categoryToInsert = new Category
            {
                ID = 1,
                Description = "Test Category",
                IdMuscleGroup = 1
            };

            // Act
            var result = await controller.InsertCategory(categoryToInsert) as CreatedResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));

            categoryRepositoryMock.Verify(repo => repo.InsertCategory(It.IsAny<Category>()), Times.Once);
        }
        [Test]
        public async Task UpdateCategory_ValidCategory_ReturnsNoContent()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var controller = new CategoryController(categoryRepositoryMock.Object);
            var categoryToUpdate = new Category { ID = 1, Description = "Updated Category", IdMuscleGroup = 1 };

            // Act
            var result = await controller.UpdateCategory(categoryToUpdate) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            categoryRepositoryMock.Verify(repo => repo.UpdateCategory(It.IsAny<Category>()), Times.Once);
        }

        [Test]
        public async Task DeleteCategory_ValidId_ReturnsNoContent()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var controller = new CategoryController(categoryRepositoryMock.Object);
            var categoryIdToDelete = 1;

            // Act
            var result = await controller.DeleteCategory(categoryIdToDelete) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            categoryRepositoryMock.Verify(repo => repo.DeleteCategory(It.IsAny<int>()), Times.Once);
        }
        [Test]
        public async Task GetMuscleGroupCategories_ValidId_ReturnsOkResult()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var controller = new CategoryController(categoryRepositoryMock.Object);
            var expectedCategories = new List<Category>
            {
                new Category { ID = 1, Description = "Category 1", IdMuscleGroup = 1 },
                new Category { ID = 2, Description = "Category 2", IdMuscleGroup = 1 },
            };


            categoryRepositoryMock.Setup(repo => repo.GetMuscleGroupCategories(It.IsAny<int>()))
                .ReturnsAsync(expectedCategories);

            // Act
            var result = await controller.GetMuscleGroupCategories(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualCategories = result.Value as IEnumerable<Category>;
            Assert.That(actualCategories, Is.EqualTo(expectedCategories));
        }
        [Test]
        public async Task GetCategory_ValidId_ReturnsOkResult()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var controller = new CategoryController(categoryRepositoryMock.Object);
            var expectedCategory = new Category { ID = 1, Description = "Category 1", IdMuscleGroup = 1 };

            categoryRepositoryMock.Setup(repo => repo.GetCategory(It.IsAny<int>()))
                .ReturnsAsync(expectedCategory);

            // Act
            var result = await controller.GetCategory(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var actualCategory = result.Value as Category;
            Assert.That(actualCategory, Is.EqualTo(expectedCategory));
        }
        [Test]
        public async Task GetCategory_ValidId_ReturnsCategory()
        {
            // Arrange
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var controller = new CategoryController(categoryRepositoryMock.Object);
            var expectedCategory = new Category { ID = 1, Description = "Test Category", IdMuscleGroup = 1 };

            categoryRepositoryMock.Setup(repo => repo.GetCategory(It.IsAny<int>()))
                .ReturnsAsync(expectedCategory);

            // Act
            var result = await controller.GetCategory(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(expectedCategory, result.Value);
            categoryRepositoryMock.Verify(repo => repo.GetCategory(It.IsAny<int>()), Times.Once);
        }
        //[Test]
        //public async Task InsertCategory_InvalidData_ReturnsBadRequest()
        //{
        //    // Arrange
        //    var categoryRepositoryMock = new Mock<ICategoryRepository>();
        //    var controller = new CategoryController(categoryRepositoryMock.Object);
        //    var categoryToInsert = new Category
        //    {
        //        Description = null,  // o string.Empty
        //    };

        //    // Act
        //    var result = await controller.InsertCategory(categoryToInsert) as BadRequestResult;

        //    // Assert
        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.StatusCode, Is.EqualTo(400));
        //    categoryRepositoryMock.Verify(repo => repo.InsertCategory(It.IsAny<Category>()), Times.Never);
        //}
        //[Test]
        //public async Task UpdateCategory_NonExistentCategory_ReturnsNotFound()
        //{
        //    // Arrange
        //    var categoryRepositoryMock = new Mock<ICategoryRepository>();
        //    var controller = new CategoryController(categoryRepositoryMock.Object);
        //    var categoryToUpdate = new Category { ID = 1, Description = "Updated Category", IdMuscleGroup = 1 };

        //    // Act
        //    var result = await controller.UpdateCategory(categoryToUpdate) as NotFoundResult;

        //    // Assert
        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.StatusCode, Is.EqualTo(404));
        //    categoryRepositoryMock.Verify(repo => repo.UpdateCategory(It.IsAny<Category>()), Times.Never);
        //}
        //[Test]
        //public async Task DeleteCategory_NonExistentCategory_ReturnsNotFound()
        //{
        //    // Arrange
        //    var categoryRepositoryMock = new Mock<ICategoryRepository>();
        //    var controller = new CategoryController(categoryRepositoryMock.Object);
        //    var categoryIdToDelete = -1;

        //    // Act
        //    var result = await controller.DeleteCategory(categoryIdToDelete) as NotFoundResult;

        //    // Assert
        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.StatusCode, Is.EqualTo(404));
        //    categoryRepositoryMock.Verify(repo => repo.DeleteCategory(It.IsAny<int>()), Times.Never);
        //}


    }
}
