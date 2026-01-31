using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Initialization;

namespace VersionedContentPOC.CMS.Data.Configurations
{
    internal class ContentConfiguration : IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            builder.HasKey(x => x.VersionId);
            builder.Property(x => x.VersionId)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1);
            builder.BuildContentDiscriminator();
            builder.HasOne(x => x.LanguageBranch)
                .WithMany(x => x.Versions)
                .HasForeignKey(x => new { x.ContentId, x.Language });
            builder.HasOne(v => v.ContentRoot)
                .WithMany()
                .HasForeignKey(x => x.ContentId);
        }
    }
}
