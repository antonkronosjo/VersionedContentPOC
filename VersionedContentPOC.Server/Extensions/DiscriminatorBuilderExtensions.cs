using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace VersionedContentPOC.Extensions
{
    public static class DiscriminatorBuilderExtensions
    {
        //public static void HasDiscriminatorValue(this DiscriminatorBuilder<Content> builder, Type entityType, string value)
        //{
        //    var method = builder.GetType().GetMethod("HasValue")?.MakeGenericMethod(entityType);
        //    if (method == null)
        //        throw new Exception("Method not found!");

        //    method.Invoke(builder, new[] { value });
        //}

        public static void HasDiscriminatorValue<TDiscriminator>(this DiscriminatorBuilder<TDiscriminator> builder, Type entityType, TDiscriminator value)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var method = builder.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .SingleOrDefault(m =>
                    m.Name == "HasValue" &&
                    m.IsGenericMethodDefinition &&
                    m.GetGenericArguments().Length == 1 &&
                    m.GetParameters().Length == 1);

            if (method == null)
                throw new InvalidOperationException("HasValue<TEntity>(TDiscriminator) not found.");

            var genericMethod = method.MakeGenericMethod(entityType);

            genericMethod.Invoke(builder, new object[] { value });
        }

        public static void RegisterContentDiscriminators<T>(this DiscriminatorBuilder<T> builder) where T : Enum
        {
            foreach (var contentType in GetAllContentTypes())
            {
                var attr = contentType.GetCustomAttribute<ContentTypeAttribute>();
                if (attr != null)
                {
                    builder.HasDiscriminatorValue<T>(contentType, (T)(object)attr.Discriminator);
                }
            }
        }

        public static IEnumerable<Type> GetAllContentTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x.IsClass &&
                    !x.IsAbstract &&
                    typeof(Content).IsAssignableFrom(x) &&
                    x.IsDefined(typeof(ContentTypeAttribute), false));
        }
    }
}
