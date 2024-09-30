using BuyAndHold.Api.Authentication.Endpoints;

namespace BuyAndHold.Api.Authentication;
public static class Setup
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        var root = app.MapGroup("");

        root.MapGroup("/auth")
            .WithTags("Authentication")
            .RequireAuthorization()
            .MapEndpoint<SignInEndpoint>()
            .MapEndpoint<SignOutEndpoint>()
            .MapEndpoint<RegisterEndpoint>()
            .MapEndpoint<MeEndpoint>();
    }

}
