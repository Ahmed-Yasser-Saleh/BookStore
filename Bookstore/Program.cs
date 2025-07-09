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
using Microsoft.AspNetCore.Identity.UI.Services;
using System.IdentityModel.Tokens.Jwt;
using Bookstore.TokenManagerService;

//using Castle.Core.Smtp;

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
           // builder.Services.AddScoped<IFormatProvider>();
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(conf =>
            {
                conf.SwaggerDoc("All", new OpenApiInfo { Title = "All APIs", Version = "v1" });
                conf.SwaggerDoc("Customers", new OpenApiInfo { Title = "Customer APIs", Version = "v1" });
                conf.SwaggerDoc("Books", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Books APIs", Version = "v1" });
                conf.SwaggerDoc("Catalogs", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Catalogs APIs", Version = "v1" });
                conf.SwaggerDoc("Authors", new OpenApiInfo { Title = "Authors APIs", Version = "v1" });
                conf.SwaggerDoc("Orders", new OpenApiInfo { Title = "Orders APIs", Version = "v1" });
                conf.SwaggerDoc("Account", new OpenApiInfo { Title = "Account APIs", Version = "v1" });
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
                //test with token in swagger
                conf.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
                conf.AddSecurityRequirement(new OpenApiSecurityRequirement
                                         {
                                            {
                                                new OpenApiSecurityScheme
                                                {
                                                    Reference = new OpenApiReference
                                                    {
                                                        Type = ReferenceType.SecurityScheme,
                                                        Id = JwtBearerDefaults.AuthenticationScheme
                                                    }
                                                },
                                                Array.Empty<string>()
                                            }
                                        });
                conf.EnableAnnotations();
            });

            builder.Services.AddIdentity<IdentityUser, IdentityRole>
                //( option =>
                //{ option.SignIn.RequireConfirmedEmail = true;// from email confirmation
                //  option.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider; //make the code generated is short 
                //}
                (options =>
                {
                    options.Password.RequireDigit = true; // Must contain at least one digit
                    options.Password.RequiredLength = 3;  // Minimum length of 3 characters
                    options.Password.RequireNonAlphanumeric = false; //not Must contain at least one special character
                    options.Password.RequireUppercase = false; // not Must contain at least one uppercase letter
                    options.Password.RequireLowercase = false;// not Must contain at least one lowercase letter)
                 //   options.SignIn.RequireConfirmedEmail = true;
                }).AddEntityFrameworkStores<BookstoreContext>().AddDefaultTokenProviders();

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
                op.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var token = context.SecurityToken.ToString();//as JwtSecurityToken;
                        if (token != null)
                        {
                            if (TokenManager.BlacklistedTokens.Contains(token)) 
                            {
                                context.Fail("This token has been revoked.");
                            }

                        }
                        return Task.CompletedTask;
                    }
                };
             }
            ); 
            //builder.Services.Configure<IdentityOptions>(options =>
            //{
            //    // Password settings
            //    options.Password.RequireDigit = true; // Must contain at least one digit
            //    options.Password.RequiredLength = 3;  // Minimum length of 3 characters
            //    options.Password.RequireNonAlphanumeric = false; //not Must contain at least one special character
            //    options.Password.RequireUppercase = false; // not Must contain at least one uppercase letter
            //    options.Password.RequireLowercase = false; // not Must contain at least one lowercase letter
            //});
            var stringpolicy = "ay7aga";
            builder.Services.AddCors(op =>
            {
                op.AddPolicy(stringpolicy,
                builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
            }
            app.UseSwagger();
            // app.UseSwaggerUI();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/All/swagger.json", "All");
                options.SwaggerEndpoint("/swagger/Customers/swagger.json", "Customers");
                options.SwaggerEndpoint("/swagger/Books/swagger.json", "Books");
                options.SwaggerEndpoint("/swagger/Catalogs/swagger.json", "Catalogs");
                options.SwaggerEndpoint("/swagger/Authors/swagger.json", "Authors");
                options.SwaggerEndpoint("/swagger/Orders/swagger.json", "Orders");
                options.SwaggerEndpoint("/swagger/Account/swagger.json", "Account");
                options.RoutePrefix = "swagger"; //default url for swagger http://localhost:port/swagger
                options.DisplayRequestDuration();
                options.DefaultModelsExpandDepth(-1);
            });
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors(stringpolicy);
            app.MapControllers();

            //allow anything inside wwwrot available via browser or client app
            app.UseStaticFiles();
            app.Run();
        }
    }
}
