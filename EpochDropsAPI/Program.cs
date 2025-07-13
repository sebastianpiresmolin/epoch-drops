using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EpochContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

app.MapMobEndpoints();

app.Run();
