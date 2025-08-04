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
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

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
        //AddSingleton
        services.AddScoped<AppDbContext>();
        //services.AddSingleton<ConsoleRepresentationService>();
        //ModelServices
        services.AddScoped<RailwayBranchRepository>();
        services.AddScoped<StationRepository>();
        services.AddScoped<TrainRouteRepository>();
        services.AddScoped<TrainRouteOnDateRepository>();
        services.AddScoped<TrainRouteOnDateOnStationRepository>();
        services.AddScoped<PassengerCarriageRepository>();
        services.AddScoped<PassengerCarriageOnTrainRouteOnDateRepository>();
        //ExecutiveServices
        services.AddScoped<TicketAvailabilityCheckService>();
        services.AddScoped<TicketAllocationService>();
        services.AddScoped<TicketBookingTimerService>();
        services.AddScoped<TicketSystemManipulationService>();
        services.AddScoped<TicketUserManipulationService>();

        services.AddScoped<TrainTripsSearchService>();
        services.AddScoped<TrainScheduleSearchService>();
        services.AddScoped<TrainSquadSearchService>();

        services.AddScoped<TrainSquadCopyService>();
        services.AddScoped<TrainScheduleCopyService>();
        //CoreServices
        services.AddScoped<FullTrainAssignementService>();
        services.AddScoped<FullTrainRouteSearchService>();
        services.AddScoped<FullTicketManagementService>();
        //ApiServices
        services.AddScoped<CarriageAssignmentRepositoryService>();
        services.AddScoped<TrainStopRepositoryService>();
        services.AddScoped<TrainRouteRepositoryService>();
        services.AddScoped<TrainRaceRepositoryService>();
        services.AddScoped<PassengerCarriageRepositoryService>();
        services.AddScoped<StationRepositoryService>();

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
            options.EnableAnnotations();
            //options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Shevchenko Railway Management System", Version = "v1" });
            options.SwaggerDoc("Admin Controllers", new OpenApiInfo
            {
                Title = "Admin Controllers",
                Version = "v1"
            });
            options.SwaggerDoc("Client Controllers", new OpenApiInfo
            {
                Title = "Client Controllers",
                Version = "v1"
            });
            options.SwaggerDoc("System Controllers", new OpenApiInfo
            {
                Title = "System Controllers",
                Version = "v1"
            });
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
        //app.UseWebSockets();
        //app.Use(async (HttpContext context, RequestDelegate next) =>
        //{
        //    HttpRequest request = context.Request;
        //    HttpResponse response = context.Response;
        //    PathString path = request.Path;
        //    if (path == "/ws/seat-status")
        //    {
        //        if (context.WebSockets.IsWebSocketRequest)
        //        {
        //            WebSocket web_socket = await context.WebSockets.AcceptWebSocketAsync();
        //            await WebSocketHandler.Handle(context, web_socket);
        //        }
        //        else
        //        {
        //            response.StatusCode = 400;
        //        }
        //    }
        //    else
        //    {
        //        await next.Invoke(context);
        //    }

        //});
        //////////////////
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            //app.UseSwaggerUI();
            app.UseSwaggerUI(options =>
            {
                //options.SwaggerEndpoint("/swagger/v1/swagger.json", "Shevchenko Railway Management System");
                options.SwaggerEndpoint("/swagger/Admin Controllers/swagger.json", "Admin Controllers");
                options.SwaggerEndpoint("/swagger/Client Controllers/swagger.json", "Client Controllers");
                options.SwaggerEndpoint("/swagger/System Controllers/swagger.json", "System Controllers");
            });
        }
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}
