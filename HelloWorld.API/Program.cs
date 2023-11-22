using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
var app = builder.Build();

var pathBase = builder.Configuration["API_PATH_BASE"];
if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.UsePathBase($"/{pathBase.TrimStart('/')}");
}

app.Use((context, next) =>
{
    context.Request.Scheme = "https";
    return next(context);
});

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseForwardedHeaders();
app.UseSwagger(opt =>
{
    opt.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        if (!httpReq.Headers.ContainsKey("X-Forwarded-Host")) return;
        string prefix = "api";
        var serverUrl = $"{httpReq.Scheme}://{httpReq.Headers["X-Forwarded-Host"]}/{prefix}";
        swagger.Servers = new List<OpenApiServer> { new() { Url = serverUrl } };
    });
});
app.UseSwaggerUI(c =>
{
    string version = Environment.GetEnvironmentVariable("BUILD_VERSION") ?? "1";
    c.SwaggerEndpoint("v1/swagger.json", $"Build Version: {version}");
});
//}
//Test
//app.UseHttpsRedirection();

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed((host) => true).AllowCredentials());

app.UseAuthorization();

app.MapControllers();

app.Run();
