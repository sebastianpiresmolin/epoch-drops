using EpochDropsAPI.handlers;

namespace EpochDropsAPI.Endpoints;

public static class MobEndpoints
{
    public static void MapMobEndpoints(this WebApplication app)
    {
        app.MapPost("/upload", UploadHandler.HandleAlternative);

        // Diagnostic endpoint to test deployment
        app.MapGet("/upload/test", () =>
        {
            Console.WriteLine("🧪 Upload test endpoint called");
            return Results.Ok(new { message = "Upload endpoint is working", timestamp = DateTime.UtcNow });
        });

        app.UseCors("AllowFrontend");
        app.MapGet("/search", QuickSearchHandler.Handle);

        app.UseCors("AllowFrontend");
        app.MapGet("/mob/{id:int}", MobDetailsHandler.Handle);

    }
}
