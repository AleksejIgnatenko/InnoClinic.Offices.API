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
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.Configure<RabbitMQSetting>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(mongoDbSettings.ConnectionString);
});

builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();

builder.Services.AddScoped<IOfficeService, OfficeService>();
builder.Services.AddScoped<IOfficeRepository, OfficeRepository>();

builder.Services.AddAutoMapper(typeof(OfficeMapperProfiles));

var app = builder.Build();

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

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.Run();
