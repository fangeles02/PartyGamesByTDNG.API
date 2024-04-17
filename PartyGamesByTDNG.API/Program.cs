using Microsoft.EntityFrameworkCore;
using PartyGamesByTDNG.API;
using PartyGamesByTDNG.API.DbContexts;
using PartyGamesByTDNG.API.SignalRHubs;

dbConnections environment = dbConnections.testserver;


var builder = WebApplication.CreateBuilder(args);

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
