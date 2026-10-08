using InfarlabSis.Application.Interfaces;
using InfarlabSis.Application.Mappings;
using InfarlabSis.Infrastructure.Persistence;
using InfarlabSis.Infrastructure.Seeders;
using InfarlabSis.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

namespace InfarlabApiProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Configuración JWT
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"]!;
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
                };
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Configuración de Swagger para Swashbuckle 10.x / OpenAPI v2.0+
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Infarlab API",
                    Version = "v1"
                });

                // Esquema de seguridad tipo HTTP Bearer
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Autenticación JWT. Pega únicamente tu token JWT aquí (sin la palabra Bearer).",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                
                c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", doc),
                        new List<string>()
                    }
                });
            });

            // Registro de AutoMapper
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            var app = builder.Build();

            //SEEDERS -- FAKERS

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                Console.WriteLine($" Base de datos: {context.Database.GetConnectionString()}");

                try
                {
                    await DbSeed.SeedAsync(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"📝 Mensaje: {ex.Message}");
                    Console.WriteLine($"🔍 InnerException: {ex.InnerException?.Message}");
                    Console.WriteLine($" StackTrace: {ex.StackTrace}");
                }
            }


            // ==========================================
            // Configuración del Pipeline HTTP
            // ==========================================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}