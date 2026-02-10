using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Initialization;
using VersionedContentPOC.CMS.Services;
using VersionedContentPOC.Server.Data.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x => x.SetCMSJsonOptions());
builder.Services.RegisterCMSServices("Data Source=app.db");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SetCMSSwaggerGenOptions());

var app = builder.Build();

app.Services.EnsureCMSDatabaseCreated();
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