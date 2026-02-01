using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Initialization;
using VersionedContentPOC.CMS.Services;
using VersionedContentPOC.Server.Data.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    var resolver = new DefaultJsonTypeInfoResolver();
    resolver.Modifiers.Add(ti =>
    {
        if (ti.Type == typeof(Content))
        {
            ti.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "contentType",
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor
            };
            foreach (var contentType in ContentTypeRegistry.GetRegisteredContentTypes())
                ti.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(contentType, contentType.Name));
        }
    });
    options.JsonSerializerOptions.TypeInfoResolver = resolver;
}); ;
builder.Services.RegisterCMSServices("Data Source=app.db");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SetCMSSwaggerGenOptions());

var app = builder.Build();

app.Services.EnsureDatabaseCreated();
app.UseDefaultFiles();
app.UseStaticFiles();

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