using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Services;

namespace VersionedContentPOC.CMS.Initialization
{
    internal static class ModelBuilderExtensions
    {
        private static readonly string _contentDiscriminator = "ContentType";
        internal static void BuildContentDiscriminator(this EntityTypeBuilder<Content> builder)
        {
            var discriminatorBuilder = builder.HasDiscriminator<string>(_contentDiscriminator);

            foreach (var type in ContentTypeRegistry.GetRegisteredContentTypes())
                discriminatorBuilder.HasValue(type, type.Name);
        }
    }
}
