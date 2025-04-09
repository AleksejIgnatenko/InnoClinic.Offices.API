using InnoClinic.Offices.API.Extensions;
using InnoClinic.Offices.API.Middlewares;
using InnoClinic.Offices.Application.MapperProfiles;
using InnoClinic.Offices.Application.Services;
using InnoClinic.Offices.DataAccess.Repositories;
using InnoClinic.Offices.Infrastructure.Mongo;
using InnoClinic.Offices.Infrastructure.RabbitMQ;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .CreateSerilog();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.Configure<RabbitMQSetting>(
    builder.Configuration.GetSection("RabbitMQ"));

// Load JWT settings
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(mongoDbSettings.ConnectionString);
});

builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();
builder.Services.AddTransient<IValidationService, ValidationService>();

builder.Services.AddScoped<IOfficeService, OfficeService>();
builder.Services.AddScoped<IOfficeRepository, OfficeRepository>();

builder.Services.AddAutoMapper(typeof(OfficeMapperProfiles));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var rabbitMQService = services.GetRequiredService<IRabbitMQService>();
    await rabbitMQService.CreateQueuesAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(x =>
{
    x.WithHeaders().AllowAnyHeader();
    x.WithOrigins("http://localhost:4000", "http://localhost:4001");
    x.WithMethods().AllowAnyMethod();
    x.AllowCredentials();
});

app.Run();
