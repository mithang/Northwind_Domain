using System;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Northwind.Application.Auth;
using Northwind.Application.Employees.Commands.CreateEmployee;
using Northwind.Application.Infrastructure;
using Northwind.Application.Infrastructure.AutoMapper;
using Northwind.Application.Interfaces;
using Northwind.Application.Products.Queries.GetProduct;
using Northwind.API.Filters;
using Northwind.Common;
using Northwind.Infrastructure;
using Northwind.Persistence;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Google;
using Northwind.API.Services;

namespace Northwind.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH"; // todo: get this from somewhere secure
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile).GetTypeInfo().Assembly);

            // Add framework services.
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IDateTime, MachineDateTime>();

            // Add MediatR
            services.AddMediatR(typeof(GetProductQueryHandler).GetTypeInfo().Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            // Add DbContext using SQL Server Provider
            services.AddDbContext<INorthwindDbContext, NorthwindDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("NorthwindDatabase")));
            //services.AddDbContext<INorthwindDbContext, NorthwindDbContext>(options =>
            //    {
            //        options.UseSqlServer(Configuration.GetConnectionString("NorthwindDatabase"), b => b.MigrationsAssembly("Northwind.Persistence"));
            //    });

            services.AddTransient<NorthwindDbContext>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                //Cấu hình cho password
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                //Cấu hình cho email
                options.User.RequireUniqueEmail = true;

                

            })
            .AddEntityFrameworkStores<NorthwindDbContext>().AddDefaultTokenProviders();


            services
                .AddMvc(options =>
                {
                    
                    //Nếu lỗi thì trả về error và stackTrace
                    options.Filters.Add(typeof(CustomExceptionFilterAttribute));

                    //Yêu cầu ứng dụng phải chạy bằng https
                    options.ReturnHttpNotAcceptable = true;
                    //Hoặc
                    //options.Filters.Add(new RequireHttpsAttribute());

                    options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                    // setupAction.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());

                    var xmlDataContractSerializerInputFormatter =
                        new XmlDataContractSerializerInputFormatter();
                    xmlDataContractSerializerInputFormatter.SupportedMediaTypes
                        .Add("application/vnd.marvin.authorwithdateofdeath.full+xml");
                    options.InputFormatters.Add(xmlDataContractSerializerInputFormatter);

                    var jsonInputFormatter = options.InputFormatters
                        .OfType<JsonInputFormatter>().FirstOrDefault();

                    if (jsonInputFormatter != null)
                    {
                        jsonInputFormatter.SupportedMediaTypes
                            .Add("application/vnd.marvin.author.full+json");
                        jsonInputFormatter.SupportedMediaTypes
                            .Add("application/vnd.marvin.authorwithdateofdeath.full+json");
                    }

                    var jsonOutputFormatter = options.OutputFormatters
                        .OfType<JsonOutputFormatter>().FirstOrDefault();

                    if (jsonOutputFormatter != null)
                    {
                        jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.marvin.hateoas+json");
                    }
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddFluentValidation()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                }); 
            //.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateCustomerCommandValidator>());

            //Add DI Validator
            services.AddTransient<IValidator<CreateEmployeeCommand>, CreateEmployeeCommandValidator>();

            // Customise default API behavour
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            //Yêu cầu chứng thực là token, mặt định là cookie và tự động sử dụng dữ liệu khi login còn token thì phải dùng bearer
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            //.AddGoogle(op=> {
            //    // IConfigurationSection googleAuthNSection =
            //    //Configuration.GetSection("Authentication:Google");
            //    // op.ClientId = googleAuthNSection["ClientId"];
            //    // op.ClientSecret = googleAuthNSection["ClientSecret"];
            //    //https://stackoverflow.com/questions/54942102/oauth2-login-to-google-api-with-asp-net-core-mvc
            //    op.ClientId = "351865068992-9n45989ahb0ot26m10clusj206a7ub4n.apps.googleusercontent.com";
            //    op.ClientSecret = "ERD5Lvjf8TpGUUnuyTGv27vo";
            //})
            //.AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
            //    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            //})
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,

                };
            }); //Nếu Api có mở trang web thì thêm hàm bên dưới
                //.AddCookie(async op => {
                //    //Nếu là web app thì cấu hình path login, path forbin, path signout... tại đây, để controller biết điều hướng chứng thực
                //    op.LoginPath = "/auth/login";
                //    op.Events = new CookieAuthenticationEvents
                //    {
                //        //Nếu chưa login thì trên api show ra 401, còn web thì trả về trang login
                //        OnRedirectToLogin = async ctx =>
                //        {
                //            if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                //            {
                //                ctx.Response.StatusCode = 401;
                //            }
                //            else
                //            {
                //                ctx.Response.Redirect(ctx.RedirectUri);
                //            }
                //            await Task.Yield();
                //        }
                //    };

            //});

            services.AddAuthorization(c =>
            {
                c.AddPolicy("NhanVien", p => p.RequireClaim("NhanVien", "All"));
                //c.AddPolicy("writeAccess", p => p.RequireClaim("scope", "writeAccess"));
            });

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"});
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description ="JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
            services.AddScoped<IAuthService, AuthService>();
            services.AddTransient<AccountService, AccountService>();
            services.AddTransient<FacebookService, FacebookService>();
            services.AddTransient<JwtHandler, JwtHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseHttpsRedirection();
            app.UseAuthentication();

            //Đã remove và thay đổi thành app.UseAuthentication()
            //app.UseIdentity();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

            app.UseMvc();
        }
    }
}