using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.Data;
using VersionedContentPOC.Server.Extensions;
using VersionedContentPOC.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IContentRepository, ContentRepository>();
builder.Services.AddTransient<IContentVersionRepository, ContentVersionRepository>();
builder.Services.AddTransient<IContentFactory, ContentFactory>();
builder.Services.AddControllers().AddContentPolymorphism();
builder.Services.AddDbContext<VersionedContentPOCContext>(options => options.UseSqlite("Data Source=app.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.UseAllOfForInheritance();
    c.UseOneOfForPolymorphism();
    c.SetContentDiscriminator();
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