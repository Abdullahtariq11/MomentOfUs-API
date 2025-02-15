using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Infrastructure.Configuration
{
    public class JournalConfiguration : IEntityTypeConfiguration<Journal>
    {
        public void Configure(EntityTypeBuilder<Journal> builder)
        {
            //Relationship with User
            builder.HasOne(x => x.Owner)
            .WithMany(j=>j.Journals)
            .HasForeignKey(j => j.OwnerID)
            .OnDelete(DeleteBehavior.Cascade);

            //Relationship with SharedJournal
            builder.HasMany(sj=>sj.sharedJournals)
            .WithOne(j=>j.Journal)
            .HasForeignKey(sj => sj.JournalId)
            .OnDelete(DeleteBehavior.Restrict);

            //Relationship with JournalEntries
            builder.HasMany(j=>j.journalEntries)
            .WithOne(j=>j.Journal)
            .HasForeignKey(je=>je.JournalId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}