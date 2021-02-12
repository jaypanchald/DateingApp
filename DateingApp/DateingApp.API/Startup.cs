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
using DateingApp.API.SignalR;
using System.Threading.Tasks;

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

            services.AddSingleton<PresenceTracker>();
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
            //services.AddCors();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                        .WithOrigins("http://localhost:4200")
                        //.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });
            services.AddSignalR();
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

                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrWhiteSpace(accessToken) &&
                                path.StartsWithSegments("/hubs"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
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
            app.UseCors();
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
            //app.UseCors(a => a.AllowAnyHeader()
            //        .AllowAnyMethod()
            //        .AllowCredentials()
            //        .WithOrigins("http://localhost:4200", "https://5702a4a5d0b6.ngrok.io")
            //        );

            
            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<PresenceHub>("/hubs/presence");
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<PresenceHub>("hubs/presence");
                endpoints.MapHub<MessageHub>("hubs/message");
            });

            app.UseMvc();

           
        }
    }
}
