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
            //        options.UseSqlServer(Configuration.GetConnectionString("NorthwindDatabase"),b => b.MigrationsAssembly("Northwind.API"));
            //        options.UseSqlServer(Configuration.GetConnectionString("NorthwindDatabase"), b => b.MigrationsAssembly("Northwind.Persistence"));
            //        options.UseSqlServer(Configuration.GetConnectionString("NorthwindDatabase"), b => b.MigrationsAssembly("Northwind.Application"));

            //    });


            
            services.AddTransient<NorthwindDbContext>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

            }).AddEntityFrameworkStores<NorthwindDbContext>().AddDefaultTokenProviders();

            
            //// jwt wire up
            //// Get options from app settings
            //var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            //// Configure JwtIssuerOptions
            //services.Configure<JwtIssuerOptions>(options =>
            //{
            //    options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
            //    options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
            //    options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            //});

            //var tokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuer = true,
            //    ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

            //    ValidateAudience = true,
            //    ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

            //    ValidateIssuerSigningKey = true,
            //    IssuerSigningKey = _signingKey,

            //    RequireExpirationTime = false,
            //    ValidateLifetime = true,
            //    ClockSkew = TimeSpan.Zero
            //};
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            //}).AddJwtBearer(configureOptions =>
            //{
            //    configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
            //    configureOptions.TokenValidationParameters = tokenValidationParameters;
            //    configureOptions.SaveToken = true;
            //});

            //// add identity
            //var builder = services.AddIdentityCore<ApplicationUser>(o =>
            //{
            //    // configure identity options
            //    o.Password.RequireDigit = false;
            //    o.Password.RequireLowercase = false;
            //    o.Password.RequireUppercase = false;
            //    o.Password.RequireNonAlphanumeric = false;
            //    o.Password.RequiredLength = 6;
            //});
            //builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            //builder.AddEntityFrameworkStores<NorthwindDbContext>().AddDefaultTokenProviders();

            services
                .AddMvc(options =>
                {
                    options.Filters.Add(typeof(CustomExceptionFilterAttribute));
                    options.ReturnHttpNotAcceptable = true;
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

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"}); });
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