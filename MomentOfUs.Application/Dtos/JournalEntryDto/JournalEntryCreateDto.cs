using MomentOfUs.Domain.Models;

namespace MomentOfUs.Application.Dtos.JournalEntryDto
{
    public record JournalEntryCreateDto(string Content, JournalEntry.MoodType Mood);
} 