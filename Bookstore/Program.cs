
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

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(conf =>
            {
                conf.SwaggerDoc("v1",
                    new OpenApiInfo {
                    Title = "Hello to BookStore Api - v1",
                    Version = "v1",
                    Description = "BookStore",
                    TermsOfService = new Uri("https://www.google.com/search?q=book+store&oq=book+store&gs_lcrp=EgZjaHJvbWUqCggAEAAYsQMYgAQyCggAEAAYsQMYgAQyCQgBEAAYChiABDIHCAIQABiABDIHCAMQABiABDIHCAQQABiABDIHCAUQABiABDIHCAYQABiABDIHCAcQABiABDIHCAgQABiABDIHCAkQABiABNIBCTI4NDdqMGoxNagCCLACAQ&sourceid=chrome&ie=UTF-8"),
                    Contact = new OpenApiContact {
                    Name = "Ahmed Yasser",
                    Email ="ahmedyasserr552@gmail.com"
                    }
                    
                    }
                    );
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
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            
            app.MapControllers();

            app.Run();
        }
    }
}
