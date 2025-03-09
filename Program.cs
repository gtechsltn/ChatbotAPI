using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecretKey"))
        };
    });


builder.Services.AddAuthorization(); 

builder.Services.AddHttpClient();

var app = builder.Build();


app.MapGet("/generate-token", () =>
{
    var token = AuthService.GenerateJwtToken();  
    return Results.Ok(new { token });
});

app.UseAuthentication();
app.UseAuthorization(); 

app.UseWebSockets();


app.MapGet("/ws", WebSocketHandler.HandleWebSocket);

app.Run();
