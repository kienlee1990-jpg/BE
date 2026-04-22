using KPITrackerAPI.Interfaces;
using KPITrackerAPI.Extensions;
using KPITrackerAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using KPITrackerAPI.Authorization;
using KPITrackerAPI.Data;
using KPITrackerAPI.Entities;
using KPITrackerAPI.Middlewares;
var builder = WebApplication.CreateBuilder(args);

#region DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
#endregion

#region DI Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IDanhMucChiTieuService, DanhMucChiTieuService>();
builder.Services.AddScoped<IDotGiaoChiTieuService, DotGiaoChiTieuService>();
builder.Services.AddScoped<IChiTietGiaoChiTieuService, ChiTietGiaoChiTieuService>();
builder.Services.AddScoped<IDonViService, DonViService>();
builder.Services.AddScoped<INhomThiDuaService, NhomThiDuaService>();
builder.Services.AddScoped<IKyBaoCaoKPIService, KyBaoCaoKPIService>();
builder.Services.AddScoped<ITheoDoiThucHienKPIService, TheoDoiThucHienKPIService>();
builder.Services.AddScoped<IDanhGiaKPIService, DanhGiaKPIService>();
builder.Services.AddScoped<ICauHinhNguongDanhGiaKPIService, CauHinhNguongDanhGiaKPIService>();
#endregion

#region JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new Exception("JWT Key is missing");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey))
    };
});
#endregion

#region Authorization Policies
builder.Services.AddAuthorization(options =>
{
    foreach (var permission in AppPermissions.All)
    {
        options.AddPolicy(permission, policy =>
            policy.Requirements.Add(new PermissionRequirement(permission)));
    }
});
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue",
        policy =>
        {
            var configuredOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>();

            if (configuredOrigins is { Length: > 0 })
            {
                policy.WithOrigins(configuredOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                return;
            }

            var clientUrl = builder.Configuration["App:ClientUrl"];
            if (!string.IsNullOrWhiteSpace(clientUrl))
            {
                policy.WithOrigins(clientUrl)
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                return;
            }

            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
#endregion

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();
var enableHttpsRedirection = builder.Configuration.GetValue<bool>("EnableHttpsRedirection");


#region Middleware

app.UseMiddleware<ExceptionMiddleware>();

if (enableHttpsRedirection)
{
    app.UseHttpsRedirection();
}

// CORS should run before authentication/authorization.
app.UseCors("AllowVue");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

#region Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    var autoMigrateOnStartup = builder.Configuration.GetValue<bool>("AutoMigrateOnStartup");
    if (autoMigrateOnStartup)
    {
        await dbContext.Database.MigrateAsync();
    }
    await SeedAdminPermissions.SeedAsync(services);
    await SeedDanhGiaNguongMacDinh.SeedAsync(services);
}
#endregion

app.Run();

