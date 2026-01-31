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
    internal class ContentRootConfiguration : IEntityTypeConfiguration<ContentRoot>
    {
        public void Configure(EntityTypeBuilder<ContentRoot> builder)
        {
            builder.HasKey(x => x.ContentId);
            builder.Property(x => x.ContentId)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1);
        }
    }
}
