using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using WordCountApi.Controllers.V1;
using WordCountApi.Services.Interfaces;

namespace WordCountApi.Tests
{
    [TestClass]
    public class WordCountControllerTests
    {
        private Mock<IWordCountService>? _serviceMock;
        private Mock<ILogger<WordCountController>>? _loggerMock;
        private WordCountController? _controller;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IWordCountService>();
            _loggerMock = new Mock<ILogger<WordCountController>>();
            _controller = new WordCountController(_serviceMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Upload_ShouldReturnBadRequest_WhenFileIsNull()
        {
            var result = await _controller.Upload(null);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Upload_ShouldReturnBadRequest_WhenFileIsEmpty()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            var result = await _controller.Upload(fileMock.Object);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Upload_ShouldReturnBadRequest_WhenFileExtensionIsInvalid()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);
            fileMock.Setup(f => f.FileName).Returns("test.pdf");
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");

            var result = await _controller.Upload(fileMock.Object);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Upload_ShouldReturnBadRequest_WhenContentTypeIsInvalid()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.ContentType).Returns("application/octet-stream");

            var result = await _controller.Upload(fileMock.Object);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Upload_ShouldReturnOk_WhenValidTxtFile()
        {
            var content = "Hello World";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.ContentType).Returns("text/plain");
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            _serviceMock.Setup(s => s.CountWordsAsync(It.IsAny<Stream>()))
                        .ReturnsAsync(new Dictionary<string, int> { { "hello", 1 }, { "world", 1 } });

            var result = await _controller.Upload(fileMock.Object);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var data = okResult.Value as Dictionary<string, int>;

            Assert.AreEqual(2, data.Count);
            Assert.IsTrue(data.ContainsKey("hello"));
        }

        [TestMethod]
        public async Task Upload_ShouldReturnServerError_OnException()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.FileName).Returns("file.txt");
            fileMock.Setup(f => f.ContentType).Returns("text/plain");
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            _serviceMock.Setup(s => s.CountWordsAsync(It.IsAny<Stream>()))
                        .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.Upload(fileMock.Object);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }
    }
}
