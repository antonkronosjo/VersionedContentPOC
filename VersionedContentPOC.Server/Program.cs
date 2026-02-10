using VersionedContentPOC.CMS.Initialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .RegisterCMSControllers()
    .AddJsonOptions(x => x.SetCMSJsonOptions());
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