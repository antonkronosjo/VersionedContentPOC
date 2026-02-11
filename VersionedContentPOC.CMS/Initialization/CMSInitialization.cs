using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using VersionedContentPOC.CMS.Controllers;
using VersionedContentPOC.CMS.Data;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Services;
using VersionedContentPOC.Controllers;

namespace VersionedContentPOC.CMS.Initialization
{
    public static class CMSInitialization
    {
        public static readonly string _contentDiscriminator = "contentType";

        public static IServiceCollection RegisterCMSServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<CMSContext>(options => options.UseSqlite(connectionString));
            services.AddTransient<IContentVersionRepository, ContentVersionRepository>();
            services.AddTransient<IContentRepository, ContentRepository>();
            services.AddTransient<IContentFactory, ContentFactory>();
            services.AddTransient<IContentPublishingService, ContentPublishingService>();
            return services;
        }

        public static void EnsureCMSDatabaseCreated(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CMSContext>();
            db.Database.EnsureCreated();
            db.Database.Migrate();
        }

        public static void SetCMSSwaggerGenOptions(this SwaggerGenOptions options)
        {
            options.UseOneOfForPolymorphism();
            options.SelectSubTypesUsing(baseType =>
            {
                if (baseType == typeof(Content)) // The base class from Project B
                {
                    return ContentTypeRegistry.GetRegisteredContentTypes(); // The types in Project A
                }
                return Enumerable.Empty<Type>();
            });
            options.SelectDiscriminatorNameUsing(type => typeof(Content).IsAssignableFrom(type) ? _contentDiscriminator : null);
            options.SelectDiscriminatorValueUsing(type => typeof(Content).IsAssignableFrom(type) ? type.Name : null);
        }

        public static void SetCMSJsonOptions(this JsonOptions options)
        {
            var resolver = new DefaultJsonTypeInfoResolver();
            resolver.Modifiers.Add(ti =>
            {
                if (ti.Type == typeof(Content))
                {
                    ti.PolymorphismOptions = new JsonPolymorphismOptions
                    {
                        TypeDiscriminatorPropertyName = _contentDiscriminator,
                        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor
                    };
                    foreach (var contentType in ContentTypeRegistry.GetRegisteredContentTypes())
                        ti.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(contentType, contentType.Name));
                }
            });
            options.JsonSerializerOptions.TypeInfoResolver = resolver;
        }

        public static IMvcBuilder RegisterCMSControllers(this IMvcBuilder builder)
        {
            return builder.AddApplicationPart(typeof(ContentManagementController).Assembly)
                .AddApplicationPart(typeof(ContentSummaryController).Assembly)
                .AddApplicationPart(typeof(ContentValidationController).Assembly)
                .AddApplicationPart(typeof(ContentController).Assembly);
        }
    }
}
