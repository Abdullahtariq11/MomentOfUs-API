using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using MomentOfUs.API.Controllers;
using MomentOfUs.Application.Dtos.JournalDto;
using MomentOfUs.Application.Service.Contracts;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Tests.Controller.Tests
{
    [TestFixture]
    public class JournalControllerTest
    {
        private Mock<IServiceManager> _mockServiceManager = null!;
        private Mock<IJournalService> _mockJournalService = null!;
        private Mock<ILogger<JournalController>> _mockLogger = null!;
        private JournalController _journalController = null!;

        [SetUp]
        public void Setup()
        {
            _mockServiceManager = new Mock<IServiceManager>();
            _mockJournalService = new Mock<IJournalService>();
            _mockLogger = new Mock<ILogger<JournalController>>();

            _mockServiceManager.Setup(s => s.JournalService).Returns(_mockJournalService.Object);

            _journalController = new JournalController(_mockServiceManager.Object);
        }

        private void SetUserContext(string userId)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            _journalController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public async Task CreateJournalAsync_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            var journalCreateDto = new JournalCreateDto("Test Journal", "Test Photo URL");
            var userId = "testUserId";
            var journalId = Guid.NewGuid();

            SetUserContext(userId);

            _mockJournalService.Setup(js => js.CreateJournalAsync(userId, journalCreateDto.Title, journalCreateDto.PhotoUrl))
                .ReturnsAsync(journalId);

            // Act
            var result = await _journalController.CreateJournal(journalCreateDto);

            // Assert
            Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
            var createdAtActionResult = (CreatedAtActionResult)result;

            Assert.That(createdAtActionResult.RouteValues!["id"], Is.EqualTo(journalId));
            _mockJournalService.Verify(js => js.CreateJournalAsync(userId, journalCreateDto.Title, journalCreateDto.PhotoUrl), Times.Once);
        }

        [Test]
        public async Task GetJournalById_ShouldReturnOkObjectResult()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var userId = "testUserId";
            var journalDto = new JournalDto(journalId, "Test Journal", "Test Photo URL");

            SetUserContext(userId);

            _mockJournalService.Setup(js => js.GetJournalByIdAsync(journalId, userId))
                .ReturnsAsync(new Journal { Id = journalId, Title = "Test Journal", PhotoUrl = "Test Photo URL" });

            // Act
            var result = await _journalController.GetJournalById(journalId);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okObjectResult = (OkObjectResult)result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<Journal>());
            _mockJournalService.Verify(js => js.GetJournalByIdAsync(journalId, userId), Times.Once);
        }

        [Test]
        public async Task UpdateJournal_ShouldReturnNoContentResult()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var userId = "testUserId";
            var journalUpdateDto = new JournalUpdateDto("Updated Journal", "Updated Photo URL");

            SetUserContext(userId);

            _mockJournalService.Setup(js => js.UpdateJournalAsync(journalId, userId, journalUpdateDto.Title, journalUpdateDto.PhotoUrl))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _journalController.UpdateJournal(journalId, journalUpdateDto);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
            _mockJournalService.Verify(js => js.UpdateJournalAsync(journalId, userId, journalUpdateDto.Title, journalUpdateDto.PhotoUrl), Times.Once);
        }

        [Test]
        public async Task DeleteJournal_ShouldReturnNoContentResult()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var userId = "testUserId";

            SetUserContext(userId);

            _mockJournalService.Setup(js => js.DeleteJournalAsync(journalId, userId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _journalController.DeleteJournal(journalId);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
            _mockJournalService.Verify(js => js.DeleteJournalAsync(journalId, userId), Times.Once);
        }

        [Test]
        public async Task GetAllJournals_ShouldReturnOkObjectResult()
        {
            // Arrange
            var userId = "testUserId";
            var journals = new List<Journal>
            {
                new Journal { Id = Guid.NewGuid(), Title = "Journal 1", PhotoUrl = "Photo URL 1" },
                new Journal { Id = Guid.NewGuid(), Title = "Journal 2", PhotoUrl = "Photo URL 2" }
            };

            SetUserContext(userId);

            _mockJournalService.Setup(js => js.GetUserJournalsAsync(userId)).ReturnsAsync(journals);

            // Act
            var result = await _journalController.GetAllJournals();

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okObjectResult = (OkObjectResult)result;
            Assert.That(okObjectResult.Value, Is.InstanceOf<IEnumerable<Journal>>());

            var returnedJournals = (IEnumerable<Journal>)okObjectResult.Value!;
            Assert.That(returnedJournals.Count(), Is.EqualTo(journals.Count));

            _mockJournalService.Verify(js => js.GetUserJournalsAsync(userId), Times.Once);
        }
    }
}