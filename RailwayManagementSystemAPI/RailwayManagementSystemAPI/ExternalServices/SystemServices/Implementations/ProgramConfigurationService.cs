using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RailwayCore.Context;
using RailwayCore.InternalServices.CoreServices.Implementations;
using RailwayCore.InternalServices.CoreServices.Interfaces;
using RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Implementations;
using RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Interfaces;
using RailwayCore.InternalServices.ExecutiveServices.TrainAssignmentServices.Implementations;
using RailwayCore.InternalServices.ExecutiveServices.TrainAssignmentServices.Interfaces;
using RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices.Implementations;
using RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices.Interfaces;
using RailwayCore.InternalServices.ModelRepositories.Implementations;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Implementations;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.TrainAssignmentServices.Implementations;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.TrainAssignmentServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.EmailServices.Implementations;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.EmailServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.Interfaces;
using System.Text;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.Implementations
{
    public class LogicalServiceConfigurationManager
    {
        public void ConfigureModelRepositories(IServiceCollection services)
        {
            services.AddScoped<IRailwayBranchRepository, RailwayBranchRepository>();
            services.AddScoped<IStationRepository, StationRepository>();
            services.AddScoped<ITrainRouteRepository, TrainRouteRepository>();
            services.AddScoped<ITrainRouteOnDateRepository, TrainRouteOnDateRepository>();
            services.AddScoped<ITrainRouteOnDateOnStationRepository, TrainRouteOnDateOnStationRepository>();
            services.AddScoped<IPassengerCarriageRepository, PassengerCarriageRepository>();
            services.AddScoped<IPassengerCarriageOnTrainRouteOnDateRepository, PassengerCarriageOnTrainRouteOnDateRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<ITicketBookingRepository, TicketBookingRepository>();    
            
            //services.AddScoped<RailwayBranchRepository>();
            //services.AddScoped<StationRepository>();
            //services.AddScoped<TrainRouteRepository>();
            //services.AddScoped<TrainRouteOnDateRepository>();
            //services.AddScoped<TrainRouteOnDateOnStationRepository>();
            //services.AddScoped<PassengerCarriageRepository>();
            //services.AddScoped<PassengerCarriageOnTrainRouteOnDateRepository>();
            //services.AddScoped<ImageRepository>();
            //services.AddScoped<TicketBookingRepository>();
        }
        public void ConfigureTicketManagementExecutiveServices(IServiceCollection services)
        {
            services.AddScoped<ITicketAvailabilityCheckService, TicketAvailabilityCheckService>();
            services.AddScoped<ITicketAllocationService, TicketAllocationService>();
            services.AddScoped<ITicketBookingTimerService, TicketBookingTimerService>();
            services.AddScoped<ITicketSystemManipulationService, TicketSystemManipulationService>();
            services.AddScoped<ITicketUserManipulationService, TicketUserManipulationService>();

            //services.AddScoped<TicketAvailabilityCheckService>();
            //services.AddScoped<TicketAllocationService>();
            //services.AddScoped<TicketBookingTimerService>();
            //services.AddScoped<TicketSystemManipulationService>();
            //services.AddScoped<TicketUserManipulationService>();
        }
        public void ConfigureTrainRouteSearchExecutiveServices(IServiceCollection services)
        {
            services.AddScoped<ITrainTripsSearchService, TrainTripsSearchService>();
            services.AddScoped<ITrainScheduleSearchService, TrainScheduleSearchService>();
            services.AddScoped<ITrainSquadSearchService, TrainSquadSearchService>();

            //services.AddScoped<TrainTripsSearchService>();
            //services.AddScoped<TrainScheduleSearchService>();
            //services.AddScoped<TrainSquadSearchService>();
        }
        public void ConfigureTrainAssignmentExecutiveServices(IServiceCollection services)
        {
            services.AddScoped<ITrainSquadCopyService, TrainSquadCopyService>();
            services.AddScoped<ITrainScheduleCopyService, TrainScheduleCopyService>();

            //services.AddScoped<TrainSquadCopyService>();
            //services.AddScoped<TrainScheduleCopyService>();
        }
        public void ConfigureCoreServices(IServiceCollection services)
        {
            services.AddScoped<IFullTrainAssignementService, FullTrainAssignementService>();
            services.AddScoped<IFullTrainRouteSearchService, FullTrainRouteSearchService>();
            services.AddScoped<IFullTicketManagementService, FullTicketManagementService>();

            //services.AddScoped<FullTrainAssignementService>();
            //services.AddScoped<FullTrainRouteSearchService>();
            //services.AddScoped<FullTicketManagementService>();
        }
        public void ConfigureModelRepositoryServices(IServiceCollection services)
        {
            services.AddScoped<ICarriageAssignmentRepositoryService, CarriageAssignmentRepositoryService>();
            services.AddScoped<ITrainStopRepositoryService, TrainStopRepositoryService>();
            services.AddScoped<ITrainRouteRepositoryService, TrainRouteRepositoryService>();
            services.AddScoped<ITrainRaceRepositoryService, TrainRaceRepositoryService>();
            services.AddScoped<IPassengerCarriageRepositoryService, PassengerCarriageRepositoryService>();
            services.AddScoped<IStationRepositoryService, StationRepositoryService>();
            services.AddScoped<ITicketBookingRepositoryService, TicketBookingRepositoryService>();
            services.AddScoped<IUserRepository, UserRepository>();

            //services.AddScoped<CarriageAssignmentRepositoryService>();
            //services.AddScoped<TrainStopRepositoryService>();
            //services.AddScoped<TrainRouteRepositoryService>();
            //services.AddScoped<TrainRaceRepositoryService>();
            //services.AddScoped<PassengerCarriageRepositoryService>();
            //services.AddScoped<StationRepositoryService>();
            //services.AddScoped<TicketBookingRepositoryService>();
        }
        public void ConfigureClientServices(IServiceCollection services)
        {
            services.AddScoped<ITrainRouteWithBookingsSearchService, TrainRouteWithBookingsSearchService>();
            services.AddScoped<ICompleteTicketBookingProcessingService, CompleteTicketBookingProcessingService>();
            services.AddScoped<IUserAccountAuthenticationService, UserAccountAuthenticationService>();
            services.AddScoped<IUserGoogleAccountAuthenticationService, UserGoogleAccountAuthenticationService>();
            services.AddScoped<IUserProfileManagementService, UserProfileManagementService>();
            services.AddScoped<IUserTicketManagementService, UserTicketManagementService>();

            //services.AddScoped<TrainRouteWithBookingsSearchService>();
            //services.AddScoped<CompleteTicketBookingProcessingService>();
            //services.AddScoped<UserAccountAuthenticationService>();
            //services.AddScoped<UserProfileManagementService>();
            //services.AddScoped<UserTicketManagementService>();
        }
        public void ConfigureAdminServices(IServiceCollection services)
        {
            services.AddScoped<IApiTrainAssignmentService, ApiTrainAssignmentService>();
        }
        public void ConfigureSystemServices(IServiceCollection services)
        {
            services.AddScoped<SystemAuthenticationService>();
            services.AddHostedService<ExpiredTicketBookingsRemovingService>();
            services.AddSingleton<IQRCodeGeneratorService, QRCodeGeneratorService>();
            services.AddScoped<IEmailTicketSender, GmailSmtpTicketSender>();
        }
    }
    public class SwaggerDocumentationConfigurationManager
    {
        public void ConfigureSwagger(IServiceCollection services)
        {
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
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter jwt-token"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
                string xml_file = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xml_path = Path.Combine(AppContext.BaseDirectory, xml_file);
                options.IncludeXmlComments(xml_path);
            });
        }
        public void UseSwagger(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/Admin Controllers/swagger.json", "Admin Controllers");
                    options.SwaggerEndpoint("/swagger/Client Controllers/swagger.json", "Client Controllers");
                    options.SwaggerEndpoint("/swagger/System Controllers/swagger.json", "System Controllers");
                });
            }
        }
    }
    public class AuthenticationAndAuthorizationConfigurationManager
    {
        public AuthenticationBuilder ConfigureGeneralAuthenticationProperties(IServiceCollection services)
        {
            AuthenticationBuilder authentication_builder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
            services.AddAuthorization();
            return authentication_builder;
        }
        public void ConfigureJwtAuthenticationAndAuthorization(AuthenticationBuilder authentication_builder, IConfiguration configuration)
        {
            authentication_builder
                .AddJwtBearer((JwtBearerOptions options) => options.TokenValidationParameters = new TokenValidationParameters
                {
                   ValidateAudience = true,
                   ValidateIssuer = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidAudience = configuration["JwtAuthentication:Audience"],
                   ValidIssuer = configuration["JwtAuthentication:Issuer"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtAuthentication:SecretKey"]!))
                });
        }
        public void ConfigureGoogleAuthentication(AuthenticationBuilder authentication_builder, IConfiguration configuration)
        {
           authentication_builder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme).AddGoogle((GoogleOptions options) =>
            {
                string? client_id = configuration["Authentication:Google:ClientId"];
                if (client_id is null)
                {
                    throw new ArgumentNullException(nameof(client_id));
                }
                string? client_secret = configuration["Authentication:Google:ClientSecret"];
                if (client_secret is null)
                {
                    throw new ArgumentNullException(nameof(client_secret));
                }
                options.ClientId = client_id;
                options.ClientSecret = client_secret;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
            });
        }
        public void UseJwtAuthenticationAndAuthorization(WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
    public class DatabaseConnectionConfiguration
    {
        public void ConfigureDbContext(IServiceCollection services)
        {
            services.AddScoped<AppDbContext>();
        }
    }
    public class WebConnectionConfiguration
    {
        public void ConfigureCorsPolicy(IServiceCollection services)
        {
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
        }
        public void UseCorsPolicy(WebApplication app)
        {
            app.UseCors("AllowFrontend");
        }
    }
    public class ProgramConfigurationService
    {
        public static LogicalServiceConfigurationManager LogicalServiceConfigurationManager = new LogicalServiceConfigurationManager();
        
        public static SwaggerDocumentationConfigurationManager SwaggerDocumentationConfigurationManager = 
            new SwaggerDocumentationConfigurationManager();
        
        public static AuthenticationAndAuthorizationConfigurationManager AuthenticationAndAuthorizationConfigurationManager = 
            new AuthenticationAndAuthorizationConfigurationManager();

        public static DatabaseConnectionConfiguration DatabaseConnectionConfiguration = new DatabaseConnectionConfiguration();

        public static WebConnectionConfiguration WebConnectionConfiguration = new WebConnectionConfiguration();
    }
}
