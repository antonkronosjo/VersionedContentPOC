using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Data.Converters
{
    internal class ContentReferenceConverter : ValueConverter<ContentReference, int>
    {
        public ContentReferenceConverter() : base
            (
                x => x.ContentId,
                x => new ContentReference(x)
            )
        {
            
        }
    }
}
