using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using MomentOfUs.Application.Service.Contracts;
using MomentOfUs.API.Controllers;
using MomentOfUs.Application.Dtos.JournalEntryDto;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Tests.Controller.Tests
{
    [TestFixture]
    public class JournalEntryControllerTests
    {
        private Mock<IServiceManager> _mockServiceManager = null!;
        private Mock<IJournalService> _mockJournalService = null!;
        private JournalEntryController _journalEntryController = null!;

        [SetUp]
        public void Setup()
        {
            _mockServiceManager = new Mock<IServiceManager>();
            _mockJournalService = new Mock<IJournalService>();

            _mockServiceManager.Setup(sm => sm.JournalService).Returns(_mockJournalService.Object);
            _journalEntryController = new JournalEntryController(_mockServiceManager.Object);

            // Simulate an authenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "testUserId")
            }, "mock"));

            _journalEntryController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task GetAllEntries_ShouldReturnOkObjectResult()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var userId = "testUserId";
            var entries = new List<JournalEntry>
            {
                new JournalEntry { Id = Guid.NewGuid(), JournalId = journalId, Content = "Content 1", Mood = JournalEntry.MoodType.Happy },
                new JournalEntry { Id = Guid.NewGuid(), JournalId = journalId, Content = "Content 2", Mood = JournalEntry.MoodType.Sad }
            };

            _mockJournalService.Setup(s => s.GetJournalEntriesAsync(journalId, userId))
                .ReturnsAsync(entries);

            // Act
            var result = await _journalEntryController.GetAllEntries(journalId);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo(entries));
            _mockJournalService.Verify(s => s.GetJournalEntriesAsync(journalId, userId), Times.Once);
        }

        [Test]
        public async Task GetEntryById_ShouldReturnOkObjectResult_WhenEntryExists()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var entryId = Guid.NewGuid();
            var userId = "testUserId";
            var entry = new JournalEntry
            {
                Id = entryId,
                JournalId = journalId,
                Content = "Sample Entry",
                Mood = JournalEntry.MoodType.Neutral
            };

            _mockJournalService.Setup(s => s.GetJournalEntryByIdAsync(journalId, entryId, userId))
                .ReturnsAsync(entry);

            // Act
            var result = await _journalEntryController.GetEntryById(journalId, entryId);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo(entry));
            _mockJournalService.Verify(s => s.GetJournalEntryByIdAsync(journalId, entryId, userId), Times.Once);
        }

        [Test]
        public async Task GetEntryById_ShouldReturnNotFound_WhenEntryDoesNotExist()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var entryId = Guid.NewGuid();
            var userId = "testUserId";

            _mockJournalService.Setup(s => s.GetJournalEntryByIdAsync(journalId, entryId, userId))
                .ReturnsAsync((JournalEntry)null!);

            // Act
            var result = await _journalEntryController.GetEntryById(journalId, entryId);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
            _mockJournalService.Verify(s => s.GetJournalEntryByIdAsync(journalId, entryId, userId), Times.Once);
        }

        [Test]
        public async Task CreateEntry_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var userId = "testUserId";
            var entryDto = new JournalEntryCreateDto("New Entry", JournalEntry.MoodType.Happy);
            var entryId = Guid.NewGuid();

            _mockJournalService.Setup(s => s.AddJournalEntryAsync(journalId, userId, entryDto.Content, entryDto.Mood))
                .ReturnsAsync(entryId);

            // Act
            var result = await _journalEntryController.CreateEntry(journalId, entryDto);

            // Assert
            Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
            var createdAtResult = (CreatedAtActionResult)result;
            Assert.That(createdAtResult.Value, Is.EqualTo(entryId));
            Assert.That(createdAtResult.ActionName, Is.EqualTo(nameof(JournalEntryController.GetEntryById)));
            _mockJournalService.Verify(s => s.AddJournalEntryAsync(journalId, userId, entryDto.Content, entryDto.Mood), Times.Once);
        }

        [Test]
        public async Task UpdateEntry_ShouldReturnNoContentResult()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var entryId = Guid.NewGuid();
            var userId = "testUserId";
            var entryDto = new JournalEntryUpdateDto("Updated Content", JournalEntry.MoodType.Angry);

            // Act
            var result = await _journalEntryController.UpdateEntry(journalId, entryId, entryDto);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
            _mockJournalService.Verify(s => s.UpdateJournalEntryAsync(journalId, entryId, userId, entryDto.Content, entryDto.Mood), Times.Once);
        }

        [Test]
        public async Task DeleteEntry_ShouldReturnNoContentResult()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var entryId = Guid.NewGuid();
            var userId = "testUserId";

            // Act
            var result = await _journalEntryController.DeleteEntry(journalId, entryId);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
            _mockJournalService.Verify(s => s.DeleteJournalEntryAsync(entryId, userId), Times.Once);
        }
    }
}