using System.Security.Claims;
using System.Text;
using Catalog.API;
using Catalog.API.Data;
using Catalog.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        BearerFormat = "JWT",
        Scheme = "bearer",
        Type = SecuritySchemeType.Http
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

builder.Services.AddDbContext<CatalogDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
    
    
builder.Services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // Define this in appsettings.json
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"], // Define this in appsettings.json
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = ClaimTypes.Role

        };
        options.SaveToken = true;
    });

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<TagsService>();
builder.Services.AddScoped<SpecificationsService>();


builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    var pendingMigrations = db.Database.GetPendingMigrations();
    if (pendingMigrations.Any())
    {
        db.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();