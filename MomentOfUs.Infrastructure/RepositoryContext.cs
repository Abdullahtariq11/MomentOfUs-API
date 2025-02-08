using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using MomentOfUs.Domain.Models;
using MomentOfUs.Infrastructure.Configuration;

namespace MomentOfUs.Infrastructure;

public class RepositoryContext : IdentityDbContext<User>
{
    public RepositoryContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This is required for identity setup
        base.OnModelCreating(modelBuilder);

        //Any configurations can be added here.
        modelBuilder.ApplyConfiguration(new UserConfiguration()); 
        modelBuilder.ApplyConfiguration(new JournalConfiguration()); 
        modelBuilder.ApplyConfiguration(new SharedJournalConfiguration()); 
        modelBuilder.ApplyConfiguration(new UserSharedJournalConfiguration()); 
        modelBuilder.ApplyConfiguration(new JournalEntryConfiguration());

    }
    ///<summary>
    /// Add Dbsets here to implement associated tables in the database. User not needed to be defined.
    /// </summary>
    /// 

    public DbSet<Journal> Journals { get; set; }
    public DbSet<SharedJournal> SharedJournals { get; set; }

    public DbSet<UserSharedJournal> UserSharedJournals { get; set;}

    public DbSet<JournalEntry> JournalEntries { get; set; }



}
