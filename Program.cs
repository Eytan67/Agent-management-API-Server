using Microsoft.EntityFrameworkCore;
using AgentManagementAPIServer.DAL;
using AgentManagementAPIServer.Services;
using AgentManagementAPIServer.Intrfaces;
using AgentManagementAPIServer.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//injection DbContext.
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MyDbContext>(options => options.UseSqlServer
    (connectionString));

//injection Agent Service.
builder.Services.AddScoped<IService<Agent>, AgentsService>();//change to singlton
builder.Services.AddScoped<IService<Target>, TargetService>();//change to singlton
builder.Services.AddScoped<IService<Mission>, MissionsService>();//change to singlton



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
