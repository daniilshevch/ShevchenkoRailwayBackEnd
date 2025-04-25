using RailwayCore.Context;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Services;
using RailwayManagementSystemAPI.AdminServices;
using RailwayManagementSystemAPI.ClientServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

class Server
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services = builder.Services;
        IConfiguration configuration = builder.Configuration;
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
        services.AddSingleton<ApiTrainAssignmentService>();
        services.AddSingleton<UserManagementService>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = configuration["JwtAuthentication:Issuer"],
                ValidAudience = configuration["JwtAuthentication:Audience"],
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtAuthentication:SecretKey"]!)),
                ValidateIssuerSigningKey = true
            };
        });
        services.AddAuthorization();
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
