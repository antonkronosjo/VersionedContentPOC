using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Services;

namespace VersionedContentPOC.CMS.Data.Configurations
{
    internal class ContentConfiguration : IEntityTypeConfiguration<Content>
    {
        private static readonly string _contentDiscriminator = "ContentType";

        public void Configure(EntityTypeBuilder<Content> builder)
        {
            builder.HasKey(x => x.VersionId);
            builder.Property(x => x.VersionId)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1);
            builder.HasOne(x => x.ContentRoot)
                .WithMany(x => x.Versions)
                .HasForeignKey(x => x.ContentId);
            BuildContentDiscriminator(builder);
        }

        private static void BuildContentDiscriminator(EntityTypeBuilder<Content> builder)
        {
            var discriminatorBuilder = builder.HasDiscriminator<string>(_contentDiscriminator);

            foreach (var type in ContentTypeRegistry.GetRegisteredContentTypes())
                discriminatorBuilder.HasValue(type, type.Name);
        }
    }
}
