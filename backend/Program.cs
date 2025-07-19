using MedicalDashboardAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//  Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  Configure file upload limits
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 100_000_000; // 100MB
});

//  Enable distributed memory cache for session
builder.Services.AddDistributedMemoryCache();

//  Configure session ( fixed for localhost & production)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; //  for localhost HTTP (use Always in production)
});

//  Configure CORS  allows frontend access with cookies)
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") //  your frontend
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//  Add MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Medical Dashboard API", Version = "v1" });
});

var app = builder.Build();

//  Swagger for development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medical Dashboard API v1");
    });
}

app.UseHttpsRedirection();          //  still needed even for HTTP local fallback

app.UseCors("Frontend");            //  MUST BE before session

app.UseSession();                   //  Session MUST be before routing

app.UseAuthorization();

app.MapControllers();

app.Run();
