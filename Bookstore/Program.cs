using Bookstore.Contex;
using Bookstore.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Bookstore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<BookstoreContext>(op => op.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("Book")));

            builder.Services.AddScoped<UnitOfwork>();
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(conf =>
            {
                conf.SwaggerDoc("All", new OpenApiInfo { Title = "All APIs", Version = "v1" });
                conf.SwaggerDoc("Admins", new OpenApiInfo { Title = "Admin APIs", Version = "v1" });
                conf.SwaggerDoc("Customers", new OpenApiInfo { Title = "Customer APIs", Version = "v1" });
                conf.SwaggerDoc("Books", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Books APIs", Version = "v1" });
                conf.SwaggerDoc("Authors", new OpenApiInfo { Title = "Authors APIs", Version = "v1" });
                conf.SwaggerDoc("Orders", new OpenApiInfo { Title = "Orders APIs", Version = "v1" });
                conf.SwaggerDoc("Loginout", new OpenApiInfo { Title = "Loginout APIs", Version = "v1" });
                conf.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var groupName = apiDesc.GroupName;
                    if (docName == "All")
                        return true;
                    return docName == groupName;
                });
                // Include XML comments for API documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    conf.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }
                conf.EnableAnnotations();
            });

            builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<BookstoreContext>();



            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(
            // validate token
            op =>
            {
                op.SaveToken = true;
                #region secret key
                string key = "My Complex Secret Key My Complex Secret Key My Complex Secret Key";
                var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
                #endregion
                op.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = secretKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };
            });
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true; // Must contain at least one digit
                options.Password.RequiredLength = 3;  // Minimum length of 3 characters
                options.Password.RequireNonAlphanumeric = false; //not Must contain at least one special character
                options.Password.RequireUppercase = false; // not Must contain at least one uppercase letter
                options.Password.RequireLowercase = false; // not Must contain at least one lowercase letter
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                // app.UseSwaggerUI();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/All/swagger.json", "All");
                    options.SwaggerEndpoint("/swagger/Admins/swagger.json", "Admins");
                    options.SwaggerEndpoint("/swagger/Customers/swagger.json", "Customers");
                    options.SwaggerEndpoint("/swagger/Books/swagger.json", "Books");
                    options.SwaggerEndpoint("/swagger/Authors/swagger.json", "Authors");
                    options.SwaggerEndpoint("/swagger/Orders/swagger.json", "Orders");
                    options.SwaggerEndpoint("/swagger/Loginout/swagger.json", "Loginout");
                    options.RoutePrefix = "swagger";
                    options.DisplayRequestDuration();
                    options.DefaultModelsExpandDepth(-1);
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            
            app.MapControllers();

            app.Run();
        }
    }
}
