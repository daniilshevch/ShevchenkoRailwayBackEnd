using Microsoft.AspNetCore.Authentication;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.Implementations;


class Server
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services = builder.Services;
        IConfiguration configuration = builder.Configuration;


        //Authentication and Authorization
        AuthenticationBuilder authentication_builder = ProgramConfigurationService.AuthenticationAndAuthorizationConfigurationManager
            .ConfigureGeneralAuthenticationProperties(services);
        ProgramConfigurationService.AuthenticationAndAuthorizationConfigurationManager
            .ConfigureJwtAuthenticationAndAuthorization(authentication_builder, configuration);
        ProgramConfigurationService.AuthenticationAndAuthorizationConfigurationManager
            .ConfigureGoogleAuthentication(authentication_builder, configuration);
        
        services.AddHttpContextAccessor();
        services.AddControllers();
        //Database
        ProgramConfigurationService.DatabaseConnectionConfiguration.ConfigureDbContext(services);
        //ModelRepositories
        ProgramConfigurationService.LogicalServiceConfigurationManager.ConfigureModelRepositories(services);
        //ExecutiveServices
        ProgramConfigurationService.LogicalServiceConfigurationManager.ConfigureTicketManagementExecutiveServices(services);
        ProgramConfigurationService.LogicalServiceConfigurationManager.ConfigureTrainRouteSearchExecutiveServices(services);
        ProgramConfigurationService.LogicalServiceConfigurationManager.ConfigureTrainAssignmentExecutiveServices(services);
        //CoreServices
        ProgramConfigurationService.LogicalServiceConfigurationManager.ConfigureCoreServices(services);
        //ApiServices
        ProgramConfigurationService.LogicalServiceConfigurationManager.ConfigureModelRepositoryServices(services);
        ProgramConfigurationService.LogicalServiceConfigurationManager.ConfigureClientServices(services);
        ProgramConfigurationService.LogicalServiceConfigurationManager.ConfigureAdminServices(services);
        ProgramConfigurationService.LogicalServiceConfigurationManager.ConfigureSystemServices(services);
        //Swagger
        ProgramConfigurationService.SwaggerDocumentationConfigurationManager.ConfigureSwagger(services);
        //Cors
        ProgramConfigurationService.WebConnectionConfiguration.ConfigureCorsPolicy(services);



        WebApplication app = builder.Build();
        
        //Authentication and Authorization
        ProgramConfigurationService.AuthenticationAndAuthorizationConfigurationManager.UseJwtAuthenticationAndAuthorization(app);
        //Cors
        ProgramConfigurationService.WebConnectionConfiguration.UseCorsPolicy(app);
        //Swagger
        ProgramConfigurationService.SwaggerDocumentationConfigurationManager.UseSwagger(app);


        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}
