using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using WordCountApi.Services.Implementations;
using WordCountApi.Services.Interfaces;

namespace WordCountApi.Tests
{
    [TestClass]
    public class WordCountServiceTests
    {
        [TestMethod]
        public async Task CountWordsAsync_ValidInput_ReturnsCorrectCounts()
        {
            // Arrange
            var sampleText = "Hello world hello HELLO";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(sampleText));

            var loggerMock = new Mock<ILogger<WordCountService>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var wordRepoMock = new Mock<IWordRepository>();

            unitOfWorkMock.SetupGet(x => x.WordRepository).Returns(wordRepoMock.Object);
            unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
            wordRepoMock.Setup(x => x.AddOrUpdateWordAsync(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            var service = new WordCountService(unitOfWorkMock.Object, loggerMock.Object);

            // Act
            var result = await service.CountWordsAsync(stream);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(3, result["hello"]);
            Assert.AreEqual(1, result["world"]);
        }

        [TestMethod]
        public async Task CountWordsAsync_EmptyStream_ReturnsEmptyDictionary()
        {
            // Arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty));

            var loggerMock = new Mock<ILogger<WordCountService>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var wordRepoMock = new Mock<IWordRepository>();

            unitOfWorkMock.SetupGet(x => x.WordRepository).Returns(wordRepoMock.Object);
            unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

            var service = new WordCountService(unitOfWorkMock.Object, loggerMock.Object);

            // Act
            var result = await service.CountWordsAsync(stream);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
