using VersionedContentPOC.CMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.CMS.Extensions;

namespace VersionedContentPOC.CMS.Data
{
    public class VersionedContentPOCContext : DbContext
    {
        public VersionedContentPOCContext(DbContextOptions<VersionedContentPOCContext> options) : base(options)
        {
            
        }

        public DbSet<ContentRoot> ContentRoots => Set<ContentRoot>();
        public DbSet<Content> Content => Set<Content>();
        public DbSet<LanguageBranch> LanguageBranches => Set<LanguageBranch>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContentRoot>(entity =>
            {
                entity.HasKey(x => x.ContentId);
                entity.Property(x => x.ContentId)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<LanguageBranch>(entity => {
                entity.HasKey(x => new { x.ContentId, x.Language });
                entity.HasOne(x => x.ActiveVersion)
                    .WithMany()
                    .HasForeignKey(x => x.ActiveVersionId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Content>(entity =>
            {
                //entity.UseTpcMappingStrategy();
                entity.HasKey(x => x.VersionId);
                entity.Property(x => x.VersionId)
                    .ValueGeneratedOnAdd();
                entity.HasOne(x => x.LanguageBranch)
                    .WithMany(x => x.Versions)
                    .HasForeignKey(x => new { x.ContentId, x.Language });
                entity.HasOne(v => v.ContentRoot)
                    .WithMany()
                    .HasForeignKey(x => x.ContentId);
            });

            modelBuilder.RegisterContentTypes();

            base.OnModelCreating(modelBuilder);
        }
    }
}
