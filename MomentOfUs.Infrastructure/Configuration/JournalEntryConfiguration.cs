using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Infrastructure.Configuration
{
    public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
    {
        public void Configure(EntityTypeBuilder<JournalEntry> builder)
        {
            // relationship with journal
            builder.HasOne(sj=>sj.Journal)
            .WithMany(j=>j.journalEntries)
            .HasForeignKey(sj=>sj.JournalId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}