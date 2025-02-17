using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MomentOfUs.Application.Service.Contracts;
using MomentOfUs.Domain.Contracts;
using MomentOfUs.Domain.Exceptions;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Application.Service
{
    public class JournalService : IJournalService
    {
        private readonly IServiceManager _serviceManager;
        private readonly ILogger<JournalService> _logger;

        private readonly IRepositoryManager _repositoryManager;

        public JournalService(IServiceManager serviceManager, ILogger<JournalService> logger, IRepositoryManager repositoryManager)
        {
            _serviceManager = serviceManager;
            _logger = logger;
            _repositoryManager = repositoryManager;
        }


        public async Task<Guid> CreateJournalAsync(string userId, string title, string? photoUrl)
        {
            _logger.LogInformation("Creating journal with Title: {title} for user with id: {userId}", title, userId);

            await ValidateUser(userId);

            var journal = new Journal
            {
                OwnerID = userId,
                Title = title,
                PhotoUrl = photoUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repositoryManager.JournalRepository.CreateAsync(journal);
            await _repositoryManager.SaveAsync();

            var fetchJournal = await _repositoryManager.JournalRepository.GetByIdAsync(journal.Id, trackChanges: false);
            if (fetchJournal == null)
            {
                throw new BadRequestException($"Failed to create {nameof(Journal)} with ID: {journal.Id}");
            }

            _logger.LogInformation("Journal with id: {journalId} created successfully", journal.Id);
            return journal.Id;
        }

        public async Task DeleteJournalAsync(Guid journalId, string userId)
        {
            await ValidateUser(userId);

            var journal = await _repositoryManager.JournalRepository.GetByIdAsync(journalId, trackChanges: true);
            CheckEntityExist(journal, journalId, nameof(journal));

            if (!await _repositoryManager.JournalRepository.IsOwnerAsync(journalId, userId))
            {
                throw new BadRequestException($"User with id{userId} does not have permission to delete the journal");
            }
            _repositoryManager.JournalRepository.Delete(journal);
            await _repositoryManager.SaveAsync();

            _logger.LogInformation("Journal with ID: {JournalId} deleted successfully", journalId);
        }

        public async Task UpdateJournalAsync(Guid journalId, string userId, string title, string? photoUrl)
        {
            await ValidateUser(userId);

            var journal = await _repositoryManager.JournalRepository.GetByIdAsync(journalId, trackChanges: true);

            CheckEntityExist(journal, journalId,nameof(journal));

            var userShared = await _repositoryManager.UserSharedJournalRepository.GetUserPermissionAsync(journalId, userId, trackChanges: false);
            var canEdit = userShared?.PermissionLevel != PermissionLevel.Edit || journal.OwnerID == userId;
            if (!canEdit)
            {
                throw new BadRequestException($"User with ID {userId} does not have permission to update this journal.");
            }

            journal.Title = title;
            journal.PhotoUrl = photoUrl;
            journal.UpdatedAt = DateTime.UtcNow;

            _repositoryManager.JournalRepository.Update(journal);
            await _repositoryManager.SaveAsync();

            _logger.LogInformation("Journal with ID: {JournalId} updated successfully", journalId);
        }

        async public Task<Journal?> GetJournalByIdAsync(Guid journalId, string userId)
        {
            await ValidateUser(userId);

            var journal = await _repositoryManager.JournalRepository.GetByIdAsync(journalId, trackChanges: false);
            CheckEntityExist(journal, journalId, nameof(journal));

            var hasAcess = await _repositoryManager.UserSharedJournalRepository.GetUserPermissionAsync(journalId, userId, trackChanges: false);
            if (hasAcess == null)
            {
                throw new BadRequestException($"User with ID {userId} does not have access to journal with ID {journalId}.");
            }

            _logger.LogInformation("Journal with ID: {JournalId} retrieved successfully", journalId);
            return journal;
        }

        public async Task<IEnumerable<Journal>> GetUserJournalsAsync(string userId)
        {
            await ValidateUser(userId);
            var journals = await _repositoryManager.JournalRepository.GetUserJournalsAsync(userId, trackChanges: false);

            if (!journals.Any())
            {
                throw new NotFoundException("No journals exists for the user");
            }
            _logger.LogInformation("Retrieved {JournalCount} journals for user ID: {UserId}", journals.Count(), userId);
            return journals;

        }

        public Task AddJournalEntryAsync(Guid journalId, string userId, string content, JournalEntry.MoodType mood)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<JournalEntry>> GetJournalEntriesAsync(Guid journalId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteJournalEntryAsync(Guid journalEntryId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task ShareJournalAsync(Guid journalId, string ownerId, string targetUserId, PermissionLevel permission)
        {
            throw new NotImplementedException();
        }


        public Task UpdateJournalEntryAsync(Guid journalEntryId, string userId, string content, JournalEntry.MoodType mood)
        {
            throw new NotImplementedException();
        }


        //Helper Methods
        private void CheckEntityExist(object? entity, Guid id, string entityName)
        {
            if (entity == null)
            {
                throw new BadRequestException($"{entityName} with ID: {id} does not exist.");
            }
        }
        private async Task ValidateUser(string userId)
        {
            if (!await _serviceManager.UserService.UserExist(userId))
            {
                throw new BadRequestException($"User with ID: {userId} does not exist.");
            }
        }
    }
}