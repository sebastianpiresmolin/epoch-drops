using EpochDropsAPI.handlers;

namespace EpochDropsAPI.Endpoints;


public static class ItemEndpoints
{
    public static void MapItemEndpoints(this WebApplication app)
    {
        app.UseCors("AllowFrontend");
        app.MapGet("/item/{id:int}", ItemDetailHandler.Handle);
    }
}