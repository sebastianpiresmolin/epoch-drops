using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;
using EpochDropsAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EpochDropsDbContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

app.MapMobEndpoints();

app.Run();
