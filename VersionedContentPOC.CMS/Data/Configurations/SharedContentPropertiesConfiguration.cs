using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Services;

namespace VersionedContentPOC.CMS.Data.Configurations
{
    internal class SharedContentPropertiesConfiguration : IEntityTypeConfiguration<SharedContentProperties>
    {
        private static readonly string _contentDiscriminator = "ContentType";

        public void Configure(EntityTypeBuilder<SharedContentProperties> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1);
            builder.HasOne(x => x.ContentRoot)
                .WithOne(x => x.SharedContentProperties)
                .HasForeignKey<SharedContentProperties>(x => x.ContentId)
                .OnDelete(DeleteBehavior.Cascade);
            BuildContentDiscriminator(builder);
        }

        private static void BuildContentDiscriminator(EntityTypeBuilder<SharedContentProperties> builder)
        {
            var discriminatorBuilder = builder.HasDiscriminator<string>(_contentDiscriminator);

            foreach (var type in ContentTypeRegistry.GetRegisteredSharedContentProperties())
                discriminatorBuilder.HasValue(type, type.Name);
        }
    }
}
