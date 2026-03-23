using CineMatch.Extensions;
using CineMatch.Services;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddTmdbClient(builder.Configuration);
builder.Services.AddOmdbClient(builder.Configuration);

builder.Services.AddScoped<IMovieApiService, MovieApiService>();

builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "Authorization by x-api-key inside request's header",
        Scheme = "ApiKeyScheme"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("ApiKey", document)] = []
    });
});

builder.Services.SetupRateLimiter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRateLimiter();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
