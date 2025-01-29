using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Infrastructure.Configuration
{
    /// <summary>
    /// Configure  relationships and seed initial data
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        { 
            //configure with Journal
            builder.HasMany(u=>u.Journals)
            .WithOne(j=>j.Owner)
            .HasForeignKey(j=>j.OwnerID)
            .OnDelete(DeleteBehavior.Cascade);

            //configure shared journal
           // builder.HasMany(u=>u.sharedJournals)
           //.WithOne(sj=>sj.)
            
        }
    }
}