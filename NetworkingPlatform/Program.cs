using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Configuration;
using NetworkingPlatform.Data;
using NetworkingPlatform.Hubs;
using NetworkingPlatform.Interface;
using NetworkingPlatform.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                      });
});


builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("AppConnectionString")));
//--
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();
builder.Services.AddIdentityCore<Users>().AddEntityFrameworkStores<AppDbContext>().AddApiEndpoints();

//--
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

//email 
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.AddSingleton<IEmailService, EmailService>();
//


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.MapHub<NotificationHub>("/notification");


app.UseRouting();
app.MapIdentityApi<Users>();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors();

app.MapControllers();

app.MapHub<NotificationHub>("/notification");

app.Run();
