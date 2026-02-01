using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using VersionedContentPOC.CMS.Data;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Initialization;
using VersionedContentPOC.CMS.Services;
using VersionedContentPOC.Server.Data.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonPolymorphism();
builder.Services.AddCMS("Data Source=app.db");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.UseAllOfForInheritance();
    c.UseOneOfForPolymorphism();
    c.SetContentDiscriminator();
    c.SchemaFilter<PolymorphicSchemaFilter>();
});

var app = builder.Build();

app.Services.EnsureDatabaseCreated();
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");
app.Run();

//public class PolymorphicSchemaFilter : ISchemaFilter
//{
//    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
//    {
//        if (context.Type == typeof(Content))
//        {
//            schema.Discriminator = new OpenApiDiscriminator
//            {
//                PropertyName = "type",
//                Mapping = new Dictionary<string, string>
//                {
//                    { nameof(NewsContent), $"#/components/schemas/{nameof(NewsContent)}" },
//                    { nameof(EventContent), $"#/components/schemas/{nameof(EventContent)}" },
//                }
//            };
//            schema.OneOf = new List<OpenApiSchema>
//                {
//                    context.SchemaGenerator.GenerateSchema(typeof(NewsContent), context.SchemaRepository),
//                    context.SchemaGenerator.GenerateSchema(typeof(EventContent), context.SchemaRepository)
//                };
//        }
//    }
//}