using EpochDropsAPI.handlers;

namespace EpochDropsAPI.Endpoints;

public static class MobEndpoints
{
    public static void MapMobEndpoints(this WebApplication app)
    {
        app.MapPost("/upload", UploadHandler.Handle);

        app.UseCors("AllowFrontend");
        app.MapGet("/search", QuickSearchHandler.Handle);
        

    }
}
