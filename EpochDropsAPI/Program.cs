using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;
using EpochDropsAPI.Endpoints;
using EpochDropsAPI.helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EpochDropsDbContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.Configure<UploaderSettings>(builder.Configuration.GetSection("Uploader"));

var app = builder.Build();

app.MapMobEndpoints();

app.Run();
