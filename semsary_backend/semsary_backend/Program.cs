
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Core;
using semsary_backend.EntityConfigurations;
using semsary_backend.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace semsary_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<Service.EmailService>();
            builder.Services.AddSingleton<Service.TokenService>();
            builder.Services.AddScoped<NotificationService>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<EntityConfigurations.ApiContext>(options=>options.UseInMemoryDatabase("semsary_db"));
            builder.Services.AddSingleton<PriceEstimator>();
            builder.Services.AddSingleton<RecommendationSystem>();

            builder.Services.Configure<CloudflareR2Settings>(builder.Configuration.GetSection("CloudflareR2"));

            builder.Services.AddSingleton<IAmazonS3>(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<CloudflareR2Settings>>().Value;

                var config = new AmazonS3Config
                {
                    ServiceURL = settings.ServiceURL,
                    ForcePathStyle = true, // Important for R2 compatibility
                    SignatureVersion="4",
                    BufferSize=8192,
                    ResignRetries=false
                   

                };

                return new AmazonS3Client(settings.AccessKey, settings.SecretKey, config);
            });
            builder.Services.AddScoped<R2StorageService>();
            builder.Services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
                ).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["token:issuer"],
                        ValidAudience = builder.Configuration["Token:issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:Key"])),
                        // Force checking that algorithm is HS256
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }

                    };
                });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddSwaggerGen(c =>
            {
                // 1. Tell Swagger about Bearer token
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter your JWT token like this: Bearer {your token here}"
                });

                // 2. Require Bearer token globally
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            builder.Services.AddHostedService<BackgroundJobs.PendingRentalRequests>();
            builder.Services.AddHostedService<BackgroundJobs.LateArrivalDateTenant>();
            var app = builder.Build();
            
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                 ApplicationDbSeeder.SeedAdminUserAsync(services);
            }
            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.
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
