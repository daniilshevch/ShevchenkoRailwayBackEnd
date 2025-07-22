using RailwayCore.Context;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.InternalServices.CoreServices;
using RailwayCore.InternalServices.ModelServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RailwayManagementSystemAPI.ExternalServices.AdminServices;
using RailwayManagementSystemAPI.ExternalServices.ClientServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
using System.Net.WebSockets;
using System.Net;
using RailwayCore.InternalServices.ExecutiveServices;
using RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices;
using RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices;

class Server
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services = builder.Services;
        IConfiguration configuration = builder.Configuration;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateAudience = true,
               ValidateIssuer = true,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidAudience = configuration["JwtAuthentication:Audience"],
               ValidIssuer = configuration["JwtAuthentication:Issuer"],
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtAuthentication:SecretKey"]!))
           });
        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddControllers();
        services.AddSingleton<AppDbContext>();
        services.AddSingleton<ConsoleRepresentationService>();
        //ModelServices
        services.AddSingleton<RailwayBranchRepository>();
        services.AddSingleton<StationRepository>();
        services.AddSingleton<TrainRouteRepository>();
        services.AddSingleton<TrainRouteOnDateRepository>();
        services.AddSingleton<TrainRouteOnDateOnStationRepository>();
        services.AddSingleton<PassengerCarriageRepository>();
        services.AddSingleton<PassengerCarriageOnTrainRouteOnDateRepository>();
        //ExecutiveServices
        services.AddSingleton<TicketAvailabilityCheckService>();
        services.AddSingleton<TicketAllocationService>();
        services.AddSingleton<TicketBookingTimerService>();
        services.AddSingleton<TicketSystemManipulationService>();
        services.AddSingleton<TicketUserManipulationService>();

        services.AddSingleton<TrainTripsSearchService>();
        services.AddSingleton<TrainScheduleSearchService>();
        services.AddSingleton<TrainSquadSearchService>();
        //CoreServices
        services.AddSingleton<FullTrainAssignementService>();
        services.AddSingleton<FullTrainRouteSearchService>();
        services.AddSingleton<FullTicketManagementService>();
        //ApiServices
        services.AddScoped<TrainRouteWithBookingsSearchService>();
        services.AddScoped<CompleteTicketBookingService>();
        services.AddScoped<ApiTrainAssignmentService>();
        services.AddScoped<UserAccountManagementService>();
        services.AddScoped<UserTicketManagementService>();
        services.AddScoped<SystemAuthenticationService>();
        services.AddHostedService<ExpiredTicketBookingsRemovingService>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Shevchenko Railway Management System", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Enter jwt-token"
            });
            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });


        });
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    policy.WithOrigins("http://localhost:50994")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });
        WebApplication app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors("AllowFrontend");
        ////////////////////////////////// Sockets
        app.UseWebSockets();
        app.Use(async (HttpContext context, RequestDelegate next) =>
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            PathString path = request.Path;
            if (path == "/ws/seat-status")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket web_socket = await context.WebSockets.AcceptWebSocketAsync();
                    await WebSocketHandler.Handle(context, web_socket);
                }
                else
                {
                    response.StatusCode = 400;
                }
            }
            else
            {
                await next.Invoke(context);
            }

        });
        //////////////////
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}
