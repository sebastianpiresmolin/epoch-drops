using EpochDropsAPI.handlers;

namespace EpochDropsAPI.Endpoints;


public static class ItemEndpoints
{
    public static void MapItemEndpoints(this WebApplication app)
    {
        app.UseCors("AllowFrontend");
        app.MapGet("/item/{id:int}", ItemDetailHandler.Handle);

        app.UseCors("AllowFrontend");
        app.MapGet("/items/by-subtype", GetItemsBySubType.Handle);

        app.UseCors("AllowFrontend");
        app.MapGet("/category/armor", GetArmorItemsBySubType.Handle);

        app.UseCors("AllowFrontend");
        app.MapGet("/category/general", GetItemsByCategory.Handle);
    }
}