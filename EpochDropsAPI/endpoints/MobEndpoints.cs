using EpochDropsAPI.Handlers;

namespace EpochDropsAPI.Endpoints;

public static class MobEndpoints
{
    public static void MapMobEndpoints(this WebApplication app)
    {
        app.MapPost("/upload", UploadHandler.Handle);
    }
}
