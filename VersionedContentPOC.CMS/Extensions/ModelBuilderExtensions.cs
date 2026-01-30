using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.CMS.Services;

namespace VersionedContentPOC.CMS.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void RegisterContentTypes(this ModelBuilder modelBuilder)
        {
            foreach (var type in ContentTypeRegistry.GetRegisteredContentTypes())
                modelBuilder.Entity(type);
        }
    }
}
