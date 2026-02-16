using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Services;

namespace VersionedContentPOC.CMS.Data.Configurations
{
    internal class InvariantVersionConfiguration : IEntityTypeConfiguration<InvariantVersion>
    {
        private static readonly string _contentDiscriminator = "ContentType";

        public void Configure(EntityTypeBuilder<InvariantVersion> builder)
        {
            builder.HasKey(x => x.VersionId);
            builder.Property(x => x.VersionId)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1);
            builder.HasOne(x => x.ContentRoot)
                .WithMany(x => x.SharedContentProperties)
                .HasForeignKey(x => x.ContentId)
                .OnDelete(DeleteBehavior.Cascade);
            BuildContentDiscriminator(builder);
        }

        private static void BuildContentDiscriminator(EntityTypeBuilder<InvariantVersion> builder)
        {
            var discriminatorBuilder = builder.HasDiscriminator<string>(_contentDiscriminator);

            foreach (var type in ContentTypeRegistry.GetRegisteredTypes().Where(x => typeof(InvariantVersion).IsAssignableFrom(x)))
                discriminatorBuilder.HasValue(type, type.Name);
        }
    }
}
