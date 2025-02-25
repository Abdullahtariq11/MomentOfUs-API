using MomentOfUs.Domain.Models;

namespace MomentOfUs.Application.Dtos.JournalEntryDto
{
    public record JournalEntryUpdateDto(string Content, JournalEntry.MoodType Mood);
} 