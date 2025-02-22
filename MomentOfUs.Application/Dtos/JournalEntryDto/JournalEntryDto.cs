using MomentOfUs.Domain.Models;

namespace MomentOfUs.Application.Dtos.JournalEntryDto
{
    public record JournalEntryDto(Guid Id, Guid JournalId, string Content, JournalEntry.MoodType Mood);
} 