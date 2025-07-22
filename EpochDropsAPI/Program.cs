using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;
using EpochDropsAPI.Endpoints;
using EpochDropsAPI.helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EpochDropsDbContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.Configure<UploaderSettings>(builder.Configuration.GetSection("Uploader"));

builder.Services.AddCors(options =>
{
   options.AddPolicy("AllowFrontend",
       policy => policy
           .WithOrigins("http://localhost:3000", "https://epoch-drops.vercel.app", "https://epoch-drops.com", "https://www.epoch-drops.com", "localhost:3000")
           .AllowAnyMethod()
           .AllowAnyHeader()
   );
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
   var db = scope.ServiceProvider.GetRequiredService<EpochDropsDbContext>();
   db.Database.Migrate();
}

app.MapMobEndpoints();
app.MapItemEndpoints();
app.MapQuestEndpoints();

app.Urls.Add("http://*:8080");

app.Run();
