using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.CMS.Data.Configurations;
using VersionedContentPOC.CMS.Data.Converters;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Data;

internal class CMSContext : DbContext
{
    public CMSContext(DbContextOptions<CMSContext> options) : base(options)
    {
        
    }

    internal DbSet<ContentRoot> ContentRoots => Set<ContentRoot>();
    internal DbSet<Content> Content => Set<Content>();
    internal DbSet<SharedContentProperties> SharedContentProperties => Set<SharedContentProperties>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ContentRootConfiguration());
        modelBuilder.ApplyConfiguration(new ContentConfiguration());
        modelBuilder.ApplyConfiguration(new SharedContentPropertiesConfiguration());

        var propertyConverterRegistry = new PropertyConverterRegistry();
        propertyConverterRegistry.AddRegisteredConverters(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
