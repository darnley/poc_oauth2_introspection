using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme)
    .AddOAuth2Introspection(options =>
    {
        options.IntrospectionEndpoint = "https://localhost/oauth2/userinfo";

        options.ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader;
        options.AuthorizationHeaderStyle = BasicAuthenticationHeaderStyle.Rfc6749;

        options.ClientId = "YXNkYXNkQGFzZGFzZC5jb20=";
        options.ClientSecret = "123123";

        options.SkipTokensWithDots = false;

        options.Events.OnAuthenticationFailed = (context) =>
        {
            context.Response.OnStarting(async () =>
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    Data = new
                    {
                        Message = context.Error?.ToString(),
                    }
                });
            });

            return Task.CompletedTask;
        };
    });

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
//});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
