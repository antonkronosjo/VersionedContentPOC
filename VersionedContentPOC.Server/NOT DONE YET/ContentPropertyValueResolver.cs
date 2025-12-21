namespace VersionedContentPOC.Server.Services
{
    public static class ContentSchemaResolver
    {
        
    }


    public class StringSchemaConverter : SchemaConverter<string>
    {
        public override ContentPropertyValueDto FromSchema(string contentProperty)
        {
            return new ContentPropertyValueDto()
            {

            };
        }

        public override string ToSchema(ContentPropertyValueDto value)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class SchemaConverter<T>
    {
        public abstract T ToSchema(ContentPropertyValueDto value);
        public abstract ContentPropertyValueDto FromSchema(T contentProperty);
    }
}
