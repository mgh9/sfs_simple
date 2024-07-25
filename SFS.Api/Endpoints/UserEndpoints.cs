using Microsoft.EntityFrameworkCore;
using SFS.Data;
using Swashbuckle.AspNetCore.Annotations;

namespace SFS.Api.Endpoints;

internal static class UserEndpoints
{
    internal static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/api/users", async (AppDbContext context, CancellationToken cancellationToken) =>
        {
            return Results.Ok(await context.Users.ToListAsync(cancellationToken: cancellationToken));
        }).WithName("GetUsers")
        .WithTags("Users")
        .WithMetadata(new SwaggerOperationAttribute("Get Users", "Get Users list"))
        .WithOpenApi();
    }
}
