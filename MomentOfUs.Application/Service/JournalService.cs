using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // ------------------- JOURNAL OPERATIONS -------------------

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
            CheckEntityExist(fetchJournal, journal.Id, nameof(Journal));

            _logger.LogInformation("Journal with id: {journalId} created successfully", journal.Id);
            return journal.Id;
        }

        public async Task<IEnumerable<Journal>> GetUserJournalsAsync(string userId)
        {
            await ValidateUser(userId);
            var journals = await _repositoryManager.JournalRepository.GetUserJournalsAsync(userId, trackChanges: false);

            if (!journals.Any())
                throw new NotFoundException("No journals exist for the user.");

            return journals;
        }

        public async Task<Journal?> GetJournalByIdAsync(Guid journalId, string userId)
        {
            await ValidateUser(userId);
            var journal = await _repositoryManager.JournalRepository.GetByIdAsync(journalId, trackChanges: false);
            CheckEntityExist(journal, journalId, nameof(Journal));

            var hasAccess = await _repositoryManager.UserSharedJournalRepository.GetUserPermissionAsync(journalId, userId, trackChanges: false);
            var isOwner = await _repositoryManager.JournalRepository.IsOwnerAsync(journalId, userId);
            if (hasAccess == null && !isOwner)
            {
                throw new BadRequestException($"User with ID {userId} does not have access to this journal.");
            }

            return journal;
        }

        public async Task UpdateJournalAsync(Guid journalId, string userId, string title, string? photoUrl)
        {
            await ValidateUser(userId);
            var journal = await _repositoryManager.JournalRepository.GetByIdAsync(journalId, trackChanges: true);
            CheckEntityExist(journal, journalId, nameof(Journal));

            var userShared = await _repositoryManager.UserSharedJournalRepository.GetUserPermissionAsync(journalId, userId, trackChanges: false);
            var canEdit = userShared?.PermissionLevel == PermissionLevel.Edit || journal.OwnerID == userId;
            if (!canEdit)
                throw new BadRequestException($"User with ID {userId} cannot update this journal.");

            journal.Title = title;
            journal.PhotoUrl = photoUrl;
            journal.UpdatedAt = DateTime.UtcNow;

            _repositoryManager.JournalRepository.Update(journal);
            await _repositoryManager.SaveAsync();
        }

        public async Task DeleteJournalAsync(Guid journalId, string userId)
        {
            await ValidateUser(userId);
            var journal = await _repositoryManager.JournalRepository.GetByIdAsync(journalId, trackChanges: true);
            CheckEntityExist(journal, journalId, nameof(Journal));

            var sharedJournals = await _repositoryManager.SharedJournalRepository.GetUserSharedJournalAsync(userId, false);
            if (sharedJournals.Any())
                throw new BadRequestException("Cannot delete journal with active shared users. Revoke access first.");

            if (!await _repositoryManager.JournalRepository.IsOwnerAsync(journalId, userId))
                throw new BadRequestException($"User with ID {userId} does not have permission to delete this journal.");

            _repositoryManager.JournalRepository.Delete(journal);
            await _repositoryManager.SaveAsync();
        }

        // ------------------- JOURNAL ENTRY OPERATIONS -------------------

        public async Task<Guid> AddJournalEntryAsync(Guid journalId, string userId, string content, JournalEntry.MoodType mood)
        {
            await ValidateUser(userId);
            var journal = await _repositoryManager.JournalRepository.GetByIdAsync(journalId, trackChanges: false);
            CheckEntityExist(journal, journalId, nameof(Journal));

            var sharedJournals = await _repositoryManager.SharedJournalRepository.GetByJournalIdAsync(journalId, trackChanges: false);
            var sharedJournal = sharedJournals.FirstOrDefault(sj => 
                sj.SharedWith.Any(sw => sw.UserId == userId));
            CheckEntityExist(sharedJournal, sharedJournal.Id, nameof(SharedJournal));

            // Use SharedJournalRepository for permission check
            var permission = await _repositoryManager.UserSharedJournalRepository.GetUserPermissionAsync(sharedJournal.Id, userId, trackChanges: false);
            var canEdit = permission?.PermissionLevel == PermissionLevel.Edit || journal.OwnerID == userId;
            
            if (!canEdit)
            {
                throw new BadRequestException($"User with ID {userId} does not have permission to add entries.");
            }

            var journalEntry = new JournalEntry
            {
                JournalId = journalId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Mood = mood
            };

            await _repositoryManager.JournalEntryRepository.CreateAsync(journalEntry);
            await _repositoryManager.SaveAsync();
            return journalEntry.Id;
        }

        public async Task<IEnumerable<JournalEntry>> GetJournalEntriesAsync(Guid journalId, string userId)
        {
            await ValidateUser(userId);
            var entries = await _repositoryManager.JournalEntryRepository.GetByJournalIdAsync(journalId, trackChanges: false);

            if (!entries.Any())
                throw new NotFoundException("No journal entries found for this journal.");

            return entries;
        }

        public async Task<JournalEntry?> GetJournalEntryByIdAsync(Guid journalEntryId, Guid journalId, string userId)
        {
            await ValidateUser(userId);
            var entry = await _repositoryManager.JournalEntryRepository.GetByIdAsync(journalEntryId, trackChanges: false);
            CheckEntityExist(entry, journalEntryId, nameof(JournalEntry));

            var journal = await _repositoryManager.JournalRepository.GetByIdAsync(journalId, trackChanges: false);
            var userShared = await _repositoryManager.UserSharedJournalRepository.GetUserPermissionAsync(journalId, userId, trackChanges: false);
            var canEdit = userShared?.PermissionLevel == PermissionLevel.Edit || journal.OwnerID == userId;
            if (!canEdit)
                throw new BadRequestException($"User with ID {userId} does not have permission to view this entry.");

            return entry;
        }

        public async Task UpdateJournalEntryAsync(Guid journalId, Guid entryId, string userId, string content, JournalEntry.MoodType mood)
        {
            var entry = await _repositoryManager.JournalEntryRepository.GetByIdAsync(entryId, trackChanges: false);
            CheckEntityExist(entry, entryId, nameof(JournalEntry));

            var journal = await _repositoryManager.JournalRepository.GetByIdAsync(journalId, trackChanges: false);
            var userShared = await _repositoryManager.UserSharedJournalRepository.GetUserPermissionAsync(journalId, userId, trackChanges: false);
            var canEdit = userShared?.PermissionLevel == PermissionLevel.Edit || journal.OwnerID == userId;
            if (!canEdit)
                throw new BadRequestException($"User with ID {userId} does not have permission to view this entry.");

            entry.Content = content;
            entry.Mood = mood;
            await _repositoryManager.SaveAsync();
        }

        public async Task DeleteJournalEntryAsync(Guid journalEntryId, string userId)
        {
            await ValidateUser(userId);
            var journalEntry = await _repositoryManager.JournalEntryRepository.GetByIdAsync(journalEntryId, trackChanges: true);
            CheckEntityExist(journalEntry, journalEntryId, nameof(JournalEntry));

            _repositoryManager.JournalEntryRepository.Delete(journalEntry);
            await _repositoryManager.SaveAsync();
        }

        // ------------------- SHARED JOURNAL OPERATIONS -------------------

        public async Task ShareJournalAsync(Guid journalId, string ownerId, string targetUserId, PermissionLevel permission)
        {
            await ValidateUser(ownerId);
            await ValidateUser(targetUserId);

            var journal = await _repositoryManager.JournalRepository.GetByIdAsync(journalId, trackChanges: false);
            CheckEntityExist(journal, journalId, nameof(Journal));

            if (journal.OwnerID != ownerId)
            {
                throw new BadRequestException($"User with ID {ownerId} is not the owner of this journal.");
            }

            var sharedJournals = await _repositoryManager.SharedJournalRepository.GetByJournalIdAsync(journalId, trackChanges: false);
            var sharedJournal = sharedJournals.FirstOrDefault(sj => 
                sj.SharedWith.Any(sw => sw.UserId == targetUserId));

            if (sharedJournal == null)
            {
                sharedJournal = new SharedJournal
                {
                    OwnerId = ownerId,
                    JournalId = journalId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _repositoryManager.SharedJournalRepository.CreateAsync(sharedJournal);
                await _repositoryManager.SaveAsync();
            }

            var existingPermission = await _repositoryManager.UserSharedJournalRepository
                .GetUserPermissionAsync(sharedJournal.Id, targetUserId, trackChanges: false);

            if (existingPermission != null)
            {
                if (existingPermission.PermissionLevel != permission)
                {
                    existingPermission.PermissionLevel = permission;
                    await _repositoryManager.UserSharedJournalRepository.Update(existingPermission);
                    await _repositoryManager.SaveAsync();
                }
                return;
            }

            var userSharedJournal = new UserSharedJournal
            {
                UserId = targetUserId,
                SharedJournalId = sharedJournal.Id,
                PermissionLevel = permission
            };

            await _repositoryManager.UserSharedJournalRepository.CreateAsync(userSharedJournal);
            await _repositoryManager.SaveAsync();
        }

        public async Task RevokeUserAccessAsync(Guid journalId, string ownerId, string targetUserId)
        {
            await ValidateUser(ownerId);
            await ValidateUser(targetUserId);

            // Check journal exists and ownership first
            var journal = await _repositoryManager.JournalRepository.GetByIdAsync(journalId, trackChanges: false);
            CheckEntityExist(journal, journalId, nameof(Journal));
            if (journal.OwnerID != ownerId)
            {
                throw new BadRequestException($"User with ID {ownerId} is not the owner of this journal.");
            }

            var sharedJournals = await _repositoryManager.SharedJournalRepository.GetByJournalIdAsync(journalId, trackChanges: false);
            var sharedJournal = sharedJournals.FirstOrDefault(sj => 
                sj.SharedWith.Any(sw => sw.UserId == targetUserId));
            
            if (sharedJournal == null)
            {
                throw new NotFoundException($"No shared access found for user ID {targetUserId}");
            }

            var success = await _repositoryManager.UserSharedJournalRepository.RevokeUserAccessAsync(sharedJournal.Id, targetUserId);
            if (!success)
            {
                throw new BadRequestException($"Failed to revoke access for user with ID {targetUserId}.");
            }

            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<UserSharedJournal>> GetSharedUsersAsync(string userId)
        {
            await ValidateUser(userId);
            var sharedJournals = await _repositoryManager.SharedJournalRepository.GetUserSharedJournalAsync(userId, trackChanges: false);
            if (!sharedJournals.Any())
                throw new NotFoundException($"No shared journals found for user ID {userId}.");

            var sharedUsers = sharedJournals.SelectMany(sj => sj.SharedWith);
            if (!sharedUsers.Any())
                throw new NotFoundException($"No shared users found for user ID {userId}.");

            return sharedUsers;
        }

        public async Task UpdateUserPermissionAsync(Guid journalId, string ownerId, string targetUserId, PermissionLevel newPermission)
        {
            await ValidateUser(ownerId);
            await ValidateUser(targetUserId);

            var journal = await _repositoryManager.JournalRepository.GetByIdAsync(journalId, trackChanges: false);
            CheckEntityExist(journal, journalId, nameof(Journal));

            if (journal.OwnerID != ownerId)
            {
                throw new BadRequestException($"User with ID {ownerId} is not the owner of this journal.");
            }

            var sharedJournals = await _repositoryManager.SharedJournalRepository.GetByJournalIdAsync(journalId, trackChanges: false);
            var sharedJournal = sharedJournals.FirstOrDefault(sj => 
                sj.SharedWith.Any(sw => sw.UserId == targetUserId));

            if (sharedJournal == null)
            {
                throw new BadRequestException($"User ID {targetUserId} does not have shared access to this journal.");
            }

            var existingPermission = await _repositoryManager.UserSharedJournalRepository
                .GetUserPermissionAsync(sharedJournal.Id, targetUserId, trackChanges: false);

            if (existingPermission == null)
            {
                throw new BadRequestException($"User ID {targetUserId} does not have shared access to this journal.");
            }

            existingPermission.PermissionLevel = newPermission;
            _repositoryManager.UserSharedJournalRepository.Update(existingPermission);
            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<SharedJournal>> GetSharedJournalsForUserAsync(string userId)
        {
            await ValidateUser(userId);

            var sharedJournals = await _repositoryManager.SharedJournalRepository.GetUserSharedJournalAsync(userId, trackChanges: false);
            if (!sharedJournals.Any())
                throw new NotFoundException("No journals have been shared with this user.");

            return sharedJournals;
        }

        // ------------------- HELPER METHODS -------------------

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
