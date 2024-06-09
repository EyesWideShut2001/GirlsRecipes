using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RecepiesByGirls.Controllers;
using System.Threading.Tasks;
using Xunit;

namespace RecepiesByGirls.Tests
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly Mock<IConfiguration> _configMock;

        public HomeControllerTests()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _configMock = new Mock<IConfiguration>();
        }

        [Fact]
        public async Task RecipeSearch_ShouldReturnErrorMessage_WhenSearchQueryIsNull()
        {
            // Arrange
            var controller = new HomeController(_configMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.RecipeSearch(null) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SearchResults", result?.ViewName);
            Assert.Equal("Please enter a recipe to search for.", result?.ViewData["ErrorMessage"]);
        }

        [Fact]
        public async Task RecipeSearch_ShouldReturnErrorMessage_WhenSearchQueryIsEmpty()
        {
            // Arrange
            var controller = new HomeController(_configMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.RecipeSearch("") as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SearchResults", result?.ViewName);
            Assert.Equal("Please enter a recipe to search for.", result?.ViewData["ErrorMessage"]);
        }
    }
}
