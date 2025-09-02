using RailwayManagementSystemAPI.ExternalServices.SystemServices;


class Server
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services = builder.Services;
        IConfiguration configuration = builder.Configuration;
        
        
        //Authentication and Authorization
        ProgramConfigurationService.AuthenticationAndAuthorizationConfigurationManager
            .ConfigureJwtAuthenticationAndAuthorization(services, configuration);
        
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
