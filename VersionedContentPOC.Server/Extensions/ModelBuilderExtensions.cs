using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Extensions
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
