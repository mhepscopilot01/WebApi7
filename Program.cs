using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi7.Data;
using WebApi7.Services;

var builder = WebApplication.CreateBuilder(args); // CreateBuilder ile WebApplication nesnesi oluşturuyoruz

// Add services to the container.

builder.Services.AddControllers(); // AddControllers ile Controller'ları ekliyoruz

builder.Services.AddScoped<IAuthService, AuthService>(); // AddScoped ile AuthService'i ekliyoruz

// Add DbContext Service (Modify placeholders as needed)

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // AddDbContext ile DataContext'i ekliyoruz

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPI 7", Version = "v1.1", Contact = new OpenApiContact { Name = "Mustafa Hepsarilar" }, Description = "SSP WebApi SignIn Service", License = new OpenApiLicense { Name = "Mayasoft Information Systems Ltd" } }); // SwaggerDoc ile Swagger'ın versiyonunu belirliyoruz

    // Optionally, add security for protected endpoints:
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
        
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    // Include XML Comments in Swagger
    var filePath = Path.Combine(AppContext.BaseDirectory, "WebAPI7.xml"); // XML dosyasının yolunu belirliyoruz
    c.IncludeXmlComments(filePath); 
    //http://localhost:5000/swagger/v1/swagger.json
});


// JWT Configuration  -- appsettings.json 'dan builder ile Jwt ayarlarını alıyoruz 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"]; // Jwt Key'i alıyoruz
        if (string.IsNullOrWhiteSpace(key)) // Eğer Jwt Key boş ise hata fırlatıyoruz
        {
            throw new Exception("JWT Key is missing in configuration. Check your appsettings.json"); // Hata fırlatıyoruz
        }

        options.TokenValidationParameters = new TokenValidationParameters // TokenValidationParameters ile Jwt Token'ın doğrulama parametrelerini belirliyoruz
        {
            ValidateIssuer = true, // ValidateIssuer ile Issuer'ı doğruluyoruz
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // ValidIssuer ile Issuer'ı belirliyoruz
            ValidateAudience = true, // ValidateAudience ile Audience'ı doğruluyoruz
            ValidAudience = builder.Configuration["Jwt:Audience"], // ValidAudience ile Audience'ı belirliyoruz
            ValidateLifetime = true, // ValidateLifetime ile Token'ın süresini doğruluyoruz
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty)), // IssuerSigningKey ile Key'i belirliyoruz
            ValidateIssuerSigningKey = true // ValidateIssuerSigningKey ile Key'i doğruluyoruz.
        };
    });

var app = builder.Build(); // Build ile uygulamayı oluşturuyoruz

// Configure the HTTP request pipeline.

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SSP SignIn WebApi v1.0.1"));
}


// Configure HTTPS Redirection - BURADA KALDIM
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


app.UseHttpsRedirection(); // UseHttpsRedirection ile Https yönlendirmesini aktif ediyoruz

app.UseAuthentication(); // UseAuthentication ile Authentication'ı aktif ediyoruz

app.UseAuthorization(); // UseAuthorization ile Authorization'ı aktif ediyoruz

app.MapControllers(); // MapControllers ile Controller'ları eşliyoruz

app.Run(); // Run ile uygulamayı çalıştırıyoruz
