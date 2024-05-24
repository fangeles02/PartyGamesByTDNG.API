using Microsoft.EntityFrameworkCore;
using PartyGamesByTDNG.API;
using PartyGamesByTDNG.API.DbContexts;
using PartyGamesByTDNG.API.SignalRHubs;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;


dbConnections environment = dbConnections.testserver;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettingssecrets.json", optional: true, reloadOnChange: true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PartyGamesByTdngContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(environment.ToString()));
});
builder.Services.AddSignalR();

builder.Services.AddCors(o => o.AddPolicy(
        "CorsPolicy",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    ));

Issuer = builder.Configuration["Jwt:Issuer"];
Key = builder.Configuration["Jwt:Key"];

//adding jwt auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //define which claim requires to check
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            //store the value in appsettings.json
            LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
            {
                var a = options.ClaimsIssuer;
                // string tokenstring = securityToken.UnsafeToString();
                // var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);
                // var claim = token.Claims.First(c => c.Type == "sample").Value;


                // bool valid = Validators.ValidateLifetime()
                // --> my custom check <--

                var convnow = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);

                if (convnow >= (expires ?? DateTime.Now))
                {
                    return false;
                }
                else
                {
                    return true;
                }


            },

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<SystemHub>("/System");
app.MapHub<HubOperationsHub>("/HubOperations");

app.UseCors("CorsPolicy");



app.Run();


public partial class Program
{
    public static string Issuer;
    public static string Key;
}