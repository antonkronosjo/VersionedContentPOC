using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Data.Configurations
{
    internal class LanguageBranchConfiguration : IEntityTypeConfiguration<LanguageBranch>
    {
        public void Configure(EntityTypeBuilder<LanguageBranch> builder)
        {
            builder.HasKey(x => new { x.ContentId, x.Language });
            builder.HasOne(x => x.ActiveVersion)
                .WithMany()
                .HasForeignKey(x => x.ActiveVersionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
