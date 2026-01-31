using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.CMS.Data;
using VersionedContentPOC.CMS.Initialization;
using VersionedContentPOC.CMS.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonPolymorphism();
builder.Services.AddCMS("Data Source=app.db");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.UseAllOfForInheritance();
    c.UseOneOfForPolymorphism();
    c.SetContentDiscriminator();
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