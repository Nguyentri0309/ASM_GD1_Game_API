using System.Text;
using GameApi_ASM_NguyenTriThinh.Data;
using GameApi_ASM_NguyenTriThinh.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Dang ky DbContext voi SQL Server, connection string ten Default
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Dang ky service, controller khong duoc dung DbContext truc tiep
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CharacterService>();

builder.Services.AddControllers();

// Cau hinh JWT Bearer
string jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Thieu cau hinh Jwt:Key");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// Swagger: 2 nhom Auth va Characters, co nut Authorize nhan JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("Auth", new OpenApiInfo { Title = "Game API - Auth", Version = "v1" });
    options.SwaggerDoc("Characters", new OpenApiInfo { Title = "Game API - Characters", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Dan token vao day, khong can go them chu Bearer",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/Auth/swagger.json", "Auth");
    options.SwaggerEndpoint("/swagger/Characters/swagger.json", "Characters");
});

app.UseHttpsRedirection();

// Thu tu bat buoc: Authentication truoc Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
