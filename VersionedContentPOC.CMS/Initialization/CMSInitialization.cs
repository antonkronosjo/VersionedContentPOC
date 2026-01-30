using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Services;

namespace VersionedContentPOC.CMS.Initialization
{
    public static class CMSInitialization
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
