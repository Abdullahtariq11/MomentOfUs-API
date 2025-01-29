using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Infrastructure;

public class RepositoryContext : IdentityDbContext<User>
{
    public RepositoryContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This is required for identity setup
        base.OnModelCreating(modelBuilder);

        //Any configurations can be added here.

    }
    ///<summary>
    /// Add Dbsets here to implement associated tables in the database. User not needed to be defined.
    /// </summary>
    /// 

    public DbSet<Journal> Journals { get; set; }
    public DbSet<SharedJournal> SharedJournals { get; set; }



}
