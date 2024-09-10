using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RailTicketApp.Data;
using RailTicketApp.RabbitMq;
using RailTicketApp.Services;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using RailTicketApp.Commands.Tickets;
using RailTicketApp.Commands.Trains;
using RailTicketApp.Commands.Stations;
using RailTicketApp.Commands.Routes;

var builder = WebApplication.CreateBuilder(args);

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "Authentication failed.");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Token validated.");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMqSettings"));

// Add services to the container.
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TrainService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<StationService>();
builder.Services.AddScoped<RouteService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<CreateTicketCommandHandler>();
builder.Services.AddScoped <DeleteTicketCommandHandler>();
builder.Services.AddScoped <BuyTicketCommandHandler>();
builder.Services.AddScoped<CreateTrainCommandHandler>();
builder.Services.AddScoped<DeleteTrainCommandHandler>();
builder.Services.AddScoped<CreateStationCommandHandler>();
builder.Services.AddScoped<DeleteStationCommandHandler>();
builder.Services.AddScoped<CreateRouteCommandHandler>();
builder.Services.AddScoped<DeleteRouteCommandHandler>();
builder.Services.AddScoped <RabbitMqSender>();
builder.Services.AddHostedService<RabbitMqTicketConsumer>();
builder.Services.AddHostedService<RabbitMqTrainConsumer>();
builder.Services.AddHostedService<RabbitMqStationConsumer>();
builder.Services.AddHostedService<RabbitMqRouteConsumer>();
builder.Services.AddSingleton<IServiceScopeFactory>(provider =>
            provider.GetRequiredService<IServiceScopeFactory>());
builder.Services.AddSingleton<ILogger<RabbitMqSender>, Logger<RabbitMqSender>>();
builder.Services.AddSingleton<RabbitMqSenderFactory>();
builder.Services.AddDbContext<DbContextClass>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Разрешите только ваше React-приложение
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // если используете куки или аутентификацию
        });
});

var app = builder.Build();

app.UseCors("AllowReactApp");

// Apply migrations at the startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DbContextClass>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
