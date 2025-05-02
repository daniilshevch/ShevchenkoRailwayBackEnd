using RailwayCore.Context;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RailwayManagementSystemAPI.ExternalServices.AdminServices;
using RailwayManagementSystemAPI.ExternalServices.ClientServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;

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
        services.AddSingleton<RailwayBranchService>();
        services.AddSingleton<StationService>();
        services.AddSingleton<TrainRouteService>();
        services.AddSingleton<TrainRouteOnDateService>();
        services.AddSingleton<TrainRouteOnDateOnStationService>();
        services.AddSingleton<PassengerCarriageService>();
        services.AddSingleton<PassengerCarriageOnTrainRouteOnDateService>();
        services.AddSingleton<FullTrainAssignementService>();
        services.AddSingleton<FullTrainRouteSearchService>();
        services.AddSingleton<FullTicketBookingService>();
        services.AddSingleton<ConsoleRepresentationService>();

        services.AddScoped<TrainRouteWithBookingsSearchService>();
        services.AddScoped<CompleteTicketBookingService>();
        services.AddScoped<ApiTrainAssignmentService>();
        services.AddScoped<UserAccountManagementService>();
        services.AddScoped<UserTicketManagementService>();
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
                    policy.WithOrigins("http://localhost:64041")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });
        WebApplication app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors("AllowFrontend");
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
