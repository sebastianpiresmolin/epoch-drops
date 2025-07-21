using EpochDropsAPI.handlers;

namespace EpochDropsAPI.Endpoints;

public static class QuestEndpoints
{
    public static void MapQuestEndpoints(this WebApplication app)
    {
        app.UseCors("AllowFrontend");
        app.MapGet("/quest/{id:int}", QuestDetailHandler.Handle);
    }
}