using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Infrastructure.Configuration
{
    public class UserSharedJournalConfiguration : IEntityTypeConfiguration<UserSharedJournal>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserSharedJournal> builder)
        {
            // relationship with shared journals
            builder.HasOne(usj=>usj.SharedJournal)
            .WithMany(sj=>sj.SharedWith)
            .HasForeignKey(sj=>sj.SharedJournalId)
            .OnDelete(DeleteBehavior.Cascade);

            //relationship with user
            builder.HasOne(usj=>usj.User)
            .WithMany(u=>u.userSharedJournals)
            .HasForeignKey(u=>u.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}