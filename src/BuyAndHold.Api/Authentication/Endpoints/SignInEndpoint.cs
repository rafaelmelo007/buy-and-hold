﻿using Microsoft.AspNetCore.Authentication.BearerToken;

namespace BuyAndHold.Api.Authentication.Endpoints;
public class SignInEndpoint : IEndpoint
{
    // End-point Map
    public static void Map(IEndpointRouteBuilder app) => app.MapPost($"/sign-in", Handle)
        .AllowAnonymous()
        .Produces<IResult>()
        .WithSummary("Sign in an existing user of the system");

    // Request / Response
    public record LoginRequest(string Email, string Password);

    // Handler
    public static async Task<IResult> Handle(
        [FromServices] IValidator<LoginRequest> validator,
        [FromServices] BuyAndHoldDbContext database,
        [FromServices] IDateTimeService dateTimeService,
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToProblems();
        }

        var user = database.Users.FirstOrDefault(x => x.Email == request.Email);
        if (user is null)
        {
            return "E-mail or password are invalid.".ToProblems();
        }

        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.Salt))
        {
            return "E-mail or password are invalid.".ToProblems();
        }

        var claimsPrincipal = new ClaimsPrincipal(
          new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Sid, user.Cpf),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.PrimarySid, user.UserId.ToString()),
            },
            BearerTokenDefaults.AuthenticationScheme
          )
        );

        var now = dateTimeService.NowUtc;
        var queryStringToken = Guid.NewGuid();
        await database.Users.Where(x => x.UserId == user.UserId)
            .ExecuteUpdateAsync(x => x.SetProperty(x2 => x2.QueryStringToken, queryStringToken)
            .SetProperty(x => x.UpdateDateUtc, now)
            .SetProperty(x => x.UpdatedBy, user.UserId));
        await database.SaveChangesAsync();


        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(4), // Set the token to expire in 4 hours
            IsPersistent = true,
        };

        return TypedResults.SignIn(claimsPrincipal, authProperties);
    }

    // Validations
    public class Validator : AbstractValidator<LoginRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .NotEmpty();
            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
