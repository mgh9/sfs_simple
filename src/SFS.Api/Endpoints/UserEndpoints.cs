using SFS.Application.Abstractions.Services;
using SFS.Persistence;
using Swashbuckle.AspNetCore.Annotations;

namespace SFS.Api.Endpoints;

internal static class UserEndpoints
{
    internal static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users");

        group.MapGet("/",
        [SwaggerOperation(Summary = "Get Users", Description = "Get `Users` list", Tags = ["Users"])]
        [SwaggerResponse(StatusCodes.Status200OK, "returns a list of users")]
        async (AppDbContext context, IUserService userService, CancellationToken cancellationToken) =>
        {
            return Results.Ok(await userService.GetAsync(cancellationToken));
        })
            .WithName("GetUsers")
            .WithOpenApi();
    }
}
