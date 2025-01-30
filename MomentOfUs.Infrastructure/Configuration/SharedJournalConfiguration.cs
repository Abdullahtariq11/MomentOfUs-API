using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Infrastructure.Configuration
{
    public class SharedJournalConfiguration : IEntityTypeConfiguration<SharedJournal>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<SharedJournal> builder)
        {
            //relationship with owner

            builder.HasOne(sj=>sj.Owner)
            .WithMany(o=>o.sharedJournals)
            .HasForeignKey(sj=>sj.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

            // relationship with shared user

            builder.HasMany(sj=>sj.SharedWith)
            .WithOne(usj=>usj.SharedJournal)
            .HasForeignKey(usj=>usj.SharedJournalId)
            .OnDelete(DeleteBehavior.Cascade);

            // relationship with journal

            builder.HasOne(sj=>sj.Journal)
            .WithMany(j=>j.sharedJournals)
            .HasForeignKey(sj=>sj.JournalId)
            .OnDelete(DeleteBehavior.Restrict);

        }
    }
}