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
           .WithOrigins("http://localhost:3000", "https://epoch-drops.vercel.app")
           .AllowAnyMethod()
           .AllowAnyHeader()
   );
});

var app = builder.Build();

app.MapMobEndpoints();
app.MapItemEndpoints();
app.MapQuestEndpoints();

app.Run();
