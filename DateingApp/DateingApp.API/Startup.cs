using Common.Injection;
using Dating.Repository.EntityContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Dating.Repository.Repository;
using AutoMapper;
using DatingApp.Mapper;
using DateingApp.FileStorage;
using DateingApp.API.Helper;
using Dating.Model.Entity;
using Microsoft.AspNetCore.Identity;
using DateingApp.API.Services;

namespace DateingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
                 options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddDbContext<DatingContext>(opts =>
            {
                opts.UseSqlServer(Configuration["ConnectionString:DatingDb"]);
            });

            services.AddDatingLibrary();
            services.AddTransient<ITokenService, TokenService>();
            
            services.AddAutoMapper(s =>
            {
                s.AddProfile<AutoMapping>();
            }, typeof(Startup));

            services.AddTransient<LogUserActivity>();
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.AddService<LogUserActivity>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddCors();
            services.Configure<CloudinarySetting>(Configuration.GetSection("CloudinarySettings"));

            //Seeder
            //services.AddTransient<Seed>();

            services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddSignInManager<SignInManager<User>>()
                .AddRoleValidator<RoleValidator<AppRole>>()
                .AddEntityFrameworkStores<DatingContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSetting:TokenKey").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            //for identity polict role
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)//, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {


                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async contex =>
                    {
                        contex.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = contex.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            contex.Response.AddApplicationError(error.Error.Message);
                            await contex.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            //seeder.SeedData();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseMvc();
        }
    }
}
