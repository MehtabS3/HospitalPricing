using HospitalPriceAPI.Controllers;
var builder = WebApplication.CreateBuilder(args);

// Add CORS policy for localhost:3000
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();
var app = builder.Build();

// Use CORS policy
app.UseCors("CorsPolicy");

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
