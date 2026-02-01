using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using VersionedContentPOC.CMS.Data;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Services;

namespace VersionedContentPOC.CMS.Initialization
{
    public static class CMSInitialization
    {
        public static readonly string _contentDiscriminator = "contentType";

        public static IServiceCollection AddCMS(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<CMSContext>(options => options.UseSqlite(connectionString));
            services.AddTransient<IContentVersionRepository, ContentVersionRepository>();
            services.AddTransient<IContentRepository, ContentRepository>();
            services.AddTransient<IContentFactory, ContentFactory>();
            return services;
        }

        public static void EnsureDatabaseCreated(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CMSContext>();
            db.Database.EnsureCreated();
            db.Database.Migrate();
        }

        public static void SetContentDiscriminator(this SwaggerGenOptions setupAction)
        {
            setupAction.SelectDiscriminatorNameUsing(type => typeof(Content).IsAssignableFrom(type) ? _contentDiscriminator : null);
            setupAction.SelectDiscriminatorValueUsing(type => typeof(Content).IsAssignableFrom(type) ? type.Name : null);
        }

        public static IMvcBuilder AddJsonPolymorphism(this IMvcBuilder builder)
        {
            return builder.AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
                {
                    Modifiers = { ti =>
                    {
                        if (ti.Type == typeof(Content))
                        {
                            ti.PolymorphismOptions = new JsonPolymorphismOptions
                            {
                                TypeDiscriminatorPropertyName = _contentDiscriminator,
                                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor
                            };

                            foreach (var derived in ContentTypeRegistry.GetRegisteredContentTypes())
                            {
                                ti.PolymorphismOptions.DerivedTypes.Add(
                                    new JsonDerivedType(derived, derived.Name)
                                );
                            }
                        }
                    }}
                };
            });
        }
    }

    public class PolymorphicSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(Content))
            {
                schema.Discriminator = new OpenApiDiscriminator
                {
                    PropertyName = CMSInitialization._contentDiscriminator,
                    Mapping = ContentTypeRegistry.GetRegisteredContentTypes()
                        .ToDictionary(x => x.Name, x => $"#/components/schemas/{x.Name}")
                };
                schema.OneOf = ContentTypeRegistry.GetRegisteredContentTypes().Select(x => 
                {
                    return context.SchemaGenerator.GenerateSchema(x, context.SchemaRepository);
                }).ToList();
            }
        }
    }
}
