using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.Data;
using VersionedContentPOC.Server.Data.Models;
using VersionedContentPOC.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IContentRepository, ContentRepository>();
builder.Services.AddTransient<IContentFactory, ContentFactory>();
builder.Services.AddControllers();
builder.Services.AddDbContext<VersionedContentPOCContext>(options => options.UseSqlite("Data Source=app.db"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.UseAllOfForInheritance();
    c.UseOneOfForPolymorphism();

    c.SelectDiscriminatorNameUsing(type => "contentType");
    c.SelectDiscriminatorValueUsing(subType =>
    {
        if (subType == typeof(NewsContent)) return nameof(NewsContent);
        if (subType == typeof(EventContent)) return nameof(EventContent);
        return null;
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VersionedContentPOCContext>();
    db.Database.EnsureCreated();
    db.Database.Migrate();
}

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
