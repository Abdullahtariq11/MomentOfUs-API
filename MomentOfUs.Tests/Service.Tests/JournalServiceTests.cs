using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MomentOfUs.Application.Service;
using MomentOfUs.Application.Service.Contracts;
using MomentOfUs.Domain.Contracts;
using MomentOfUs.Domain.Exceptions;
using MomentOfUs.Domain.Models;
using Moq;
using NUnit.Framework;

namespace MomentOfUs.Tests.Service.Tests
{
    [TestFixture]
    public class JournalServiceTests
    {
        private Mock<IServiceManager> _mockServiceManager = null!;
        private Mock<IRepositoryManager> _mockRepositoryManager = null!;
        private Mock<IUserService> _mockUserService = null!;
        private Mock<IJournalRepository> _mockJournalRepository = null!;
        private Mock<IJournalEntryRepository> _mockJournalEntryRepository = null!;
        private Mock<ISharedJournalRepository> _mockSharedJournalRepository = null!;
        private Mock<IUserSharedJournalRepository> _mockUserSharedJournalRepository = null!;
        private Mock<ILogger<JournalService>> _mockLogger = null!;
        private JournalService _journalService = null!;

        [SetUp]
        public void Setup()
        {
            _mockServiceManager = new Mock<IServiceManager>();
            _mockRepositoryManager = new Mock<IRepositoryManager>();
            _mockUserService = new Mock<IUserService>();
            _mockJournalRepository = new Mock<IJournalRepository>();
            _mockJournalEntryRepository = new Mock<IJournalEntryRepository>();
            _mockSharedJournalRepository = new Mock<ISharedJournalRepository>();
            _mockUserSharedJournalRepository = new Mock<IUserSharedJournalRepository>();
            _mockLogger = new Mock<ILogger<JournalService>>();

            _mockServiceManager.Setup(s => s.UserService).Returns(_mockUserService.Object);
            _mockRepositoryManager.Setup(r => r.JournalRepository).Returns(_mockJournalRepository.Object);
            _mockRepositoryManager.Setup(r => r.JournalEntryRepository).Returns(_mockJournalEntryRepository.Object);
            _mockRepositoryManager.Setup(r => r.SharedJournalRepository).Returns(_mockSharedJournalRepository.Object);
            _mockRepositoryManager.Setup(r => r.UserSharedJournalRepository).Returns(_mockUserSharedJournalRepository.Object);

            _journalService = new JournalService(_mockServiceManager.Object, _mockLogger.Object, _mockRepositoryManager.Object);
        }

        [Test]
        public async Task CreateJournalAsync_ShouldCreateJournal_WhenUserExists()
        {
            // Arrange
            var userId = "user123";
            var title = "Test Journal";
            var photoUrl = "http://photo.url";
            var journalId = Guid.NewGuid(); // Generate a valid journal ID

            // Ensure the user exists
            _mockUserService.Setup(u => u.UserExist(userId)).ReturnsAsync(true);

            // Mock journal creation
            var createdJournal = new Journal { Id = journalId, OwnerID = userId, Title = title, PhotoUrl = photoUrl };
            _mockJournalRepository.Setup(r => r.CreateAsync(It.IsAny<Journal>())).Callback<Journal>(j => j.Id = journalId).Returns(Task.CompletedTask);

            // Mock retrieving the created journal
            _mockJournalRepository.Setup(r => r.GetByIdAsync(journalId, false)).ReturnsAsync(createdJournal);

            // Mock saving changes
            _mockRepositoryManager.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _journalService.CreateJournalAsync(userId, title, photoUrl);

            // Assert
            Assert.That(result, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result, Is.EqualTo(journalId));
            _mockJournalRepository.Verify(r => r.CreateAsync(It.IsAny<Journal>()), Times.Once);
            _mockRepositoryManager.Verify(r => r.SaveAsync(), Times.Once);
        }



        [Test]
        public async Task GetUserJournalsAsync_ShouldReturnJournals_WhenUserExists()
        {
            var userId = "user123";
            var journals = new List<Journal> { new Journal { Id = Guid.NewGuid(), OwnerID = userId } };

            _mockUserService.Setup(u => u.UserExist(userId)).ReturnsAsync(true);
            _mockJournalRepository.Setup(r => r.GetUserJournalsAsync(userId, false)).ReturnsAsync(journals);

            var result = await _journalService.GetUserJournalsAsync(userId);

            Assert.That(result, Is.Not.Empty);
            Assert.That(result.First().OwnerID, Is.EqualTo(userId));
        }

        [Test]
        public async Task DeleteJournalAsync_ShouldThrowException_WhenNotOwner()
        {
            var userId = "user123";
            var journalId = Guid.NewGuid();

            _mockUserService.Setup(u => u.UserExist(userId)).ReturnsAsync(true);
            _mockJournalRepository.Setup(r => r.GetByIdAsync(journalId, true)).ReturnsAsync(new Journal { Id = journalId, OwnerID = "differentUser" });
            _mockJournalRepository.Setup(r => r.IsOwnerAsync(journalId, userId)).ReturnsAsync(false);

            Assert.ThrowsAsync<BadRequestException>(async () => await _journalService.DeleteJournalAsync(journalId, userId));
        }

        [Test]
        public async Task AddJournalEntryAsync_ShouldThrowException_WhenNoEditPermissions()
        {
            var journalId = Guid.NewGuid();
            var userId = "user123";

            _mockUserService.Setup(u => u.UserExist(userId)).ReturnsAsync(true);
            _mockJournalRepository.Setup(r => r.GetByIdAsync(journalId, false)).ReturnsAsync(new Journal { Id = journalId });
            _mockUserSharedJournalRepository.Setup(u => u.GetUserPermissionAsync(journalId, userId, false))
                .ReturnsAsync(new UserSharedJournal { PermissionLevel = PermissionLevel.View });

            Assert.ThrowsAsync<BadRequestException>(async () => await _journalService.AddJournalEntryAsync(journalId, userId, "content", JournalEntry.MoodType.Happy));
        }

        [Test]
        public async Task ShareJournalAsync_ShouldUpdatePermission_WhenAlreadyShared()
        {
            var journalId = Guid.NewGuid();
            var ownerId = "owner123";
            var targetUserId = "targetUser";

            _mockUserService.Setup(u => u.UserExist(ownerId)).ReturnsAsync(true);
            _mockUserService.Setup(u => u.UserExist(targetUserId)).ReturnsAsync(true);
            _mockJournalRepository.Setup(r => r.GetByIdAsync(journalId, false)).ReturnsAsync(new Journal { Id = journalId });
            _mockUserSharedJournalRepository.Setup(u => u.GetUserPermissionAsync(journalId, targetUserId, false))
                .ReturnsAsync(new UserSharedJournal { PermissionLevel = PermissionLevel.View });

            await _journalService.ShareJournalAsync(journalId, ownerId, targetUserId, PermissionLevel.Edit);

            _mockUserSharedJournalRepository.Verify(u => u.Update(It.IsAny<UserSharedJournal>()), Times.Once);
            _mockRepositoryManager.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task RevokeUserAccessAsync_ShouldThrowException_WhenNotOwner()
        {
            var journalId = Guid.NewGuid();
            var ownerId = "owner123";
            var targetUserId = "targetUser";

            _mockUserService.Setup(u => u.UserExist(ownerId)).ReturnsAsync(true);
            _mockUserService.Setup(u => u.UserExist(targetUserId)).ReturnsAsync(true);
            _mockJournalRepository.Setup(r => r.GetByIdAsync(journalId, false)).ReturnsAsync(new Journal { Id = journalId, OwnerID = "differentOwner" });

            Assert.ThrowsAsync<BadRequestException>(async () => await _journalService.RevokeUserAccessAsync(journalId, ownerId, targetUserId));
        }
    }
}
