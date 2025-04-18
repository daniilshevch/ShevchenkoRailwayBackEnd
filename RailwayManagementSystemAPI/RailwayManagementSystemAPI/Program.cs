using RailwayCore.Context;
using RailwayCore.Services;
using RailwayManagementSystemAPI.AdminServices;
using RailwayManagementSystemAPI.ClientServices;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddSingleton<RailwayBranchService>();
builder.Services.AddSingleton<StationService>();
builder.Services.AddSingleton<TrainRouteService>();
builder.Services.AddSingleton<TrainRouteOnDateService>();
builder.Services.AddSingleton<TrainRouteOnDateOnStationService>();
builder.Services.AddSingleton<PassengerCarriageService>();
builder.Services.AddSingleton<PassengerCarriageOnTrainRouteOnDateService>();
builder.Services.AddSingleton<FullTrainAssignementService>();
builder.Services.AddSingleton<FullTrainRouteSearchService>();
builder.Services.AddSingleton<TicketBookingService>();
builder.Services.AddSingleton<ConsoleRepresentationService>();
builder.Services.AddScoped<TrainRouteWithBookingsSearchService>();
builder.Services.AddScoped<CompleteTicketBookingService>();
builder.Services.AddSingleton<ApiTrainAssignmentService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:64041") 
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
var app = builder.Build();
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
