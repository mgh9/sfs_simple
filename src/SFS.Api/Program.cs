using SFS.Api.Endpoints;
using SFS.Api.Middlewares;
using SFS.Application;
using SFS.Domain.Mapping;
using SFS.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.EnableAnnotations();
});

builder.Services.AddApplicationServices();
builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddAutoMapper(typeof(DomainMappingProfile));

var app = builder.Build();

// Ensure the database is created and migrated to the latest version
DatabaseInitializer.Initialize(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapProductEndpoints();
app.MapUserEndpoints();

app.Run();

