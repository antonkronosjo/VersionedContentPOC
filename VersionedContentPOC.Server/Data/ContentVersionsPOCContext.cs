using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;
using System.Reflection.Emit;

namespace VersionedContentPOC.Data
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
                entity.HasKey(x => x.VersionId);
                entity.HasOne(x => x.LanguageBranch)
                    .WithMany(x => x.Versions)
                    .HasForeignKey(x => new { x.ContentId, x.Language });
                entity.HasOne(v => v.ContentRoot)
                    .WithMany()
                    .HasForeignKey(x => x.ContentId);
                entity.HasDiscriminator<string>("ContentTypeDiscriminator");
            });

            modelBuilder.RegisterContentTypes();

            base.OnModelCreating(modelBuilder);
        }
    }
}
