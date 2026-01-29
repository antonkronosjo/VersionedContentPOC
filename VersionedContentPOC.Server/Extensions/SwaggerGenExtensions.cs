using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Server.Extensions
{
    public static class SwaggerGenExtensions
    {
        private static string _contentDiscriminator = "contentType";

        public static void SetContentDiscriminator(this SwaggerGenOptions setupAction)
        {
            setupAction.SelectDiscriminatorNameUsing(type => typeof(Content).IsAssignableFrom(type) ? _contentDiscriminator : null);
            setupAction.SelectDiscriminatorValueUsing(type => typeof(Content).IsAssignableFrom(type) ? type.Name : null);
        }

        /// <summary>
        /// Registers polymorphic serialization for Content and all its derived types dynamically
        /// using ContentTypeRegistry. No attributes are required.
        /// </summary>
        public static IMvcBuilder AddContentPolymorphism(this IMvcBuilder builder)
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
}
