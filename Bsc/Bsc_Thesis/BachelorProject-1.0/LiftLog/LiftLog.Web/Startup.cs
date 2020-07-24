// Startup.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Threading.Tasks;
using AutoMapper;
using LiftLog.Core.Dto.PortableDto;
using LiftLog.Core.Enums;
using LiftLog.Data;
using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using LiftLog.Data.Repositories;
using LiftLog.Service.EmailService;
using LiftLog.Service.FileProviders;
using LiftLog.Service.Interfaces;
using LiftLog.Service.UserServices;
using LiftLog.Web.Mappings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace LiftLog.Web
{
    /// <summary>
    ///     Startup configures the request pipeline that handles all requests made to the application (dependency injection
    ///     container).
    /// </summary>
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Environment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
                builder.AddUserSecrets<Startup>();
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Environment { get; }

        /// <summary>
        ///     This method gets called by the runtime before configure to add services to the container.
        /// </summary>
        /// <param name="services"> Service collection to contain dependencies. </param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add configuration 
            services.AddSingleton(Configuration);

            //tempdata services
            services.AddMemoryCache();
            services.AddSession( /* options go here */);

            //Add EF services  
            services.AddScoped<IContext, Context>();
            services.AddDbContext<Context>(options =>
            {
                var connectionString = Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });

            // Add identity services to services container
            services.AddIdentity<User, IdentityRole<int>>()
                .AddEntityFrameworkStores<Context, int>()
                .AddDefaultTokenProviders(); // Tokens for password reset or email change 

            // Configure identity settings 
            services.Configure<IdentityOptions>(options =>
            {
                // User settings
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

                // Password settings 
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // Cookie settings
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromMinutes(60); //TimeSpan.FromDays(150);
                options.Cookies.ApplicationCookie.LoginPath = "/Account/Login";
                options.Cookies.ApplicationCookie.LogoutPath = "/Account/Logout";
                options.Cookies.ApplicationCookie.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookies.ApplicationCookie.AutomaticAuthenticate = true;
                options.Cookies.ApplicationCookie.AutomaticChallenge = true;

                // Enable cookie authentication
                options.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = async ctx =>
                    {
                        // If Web API request OK, set response 401 Unauthorized //
                        if (ctx.Request.Path.StartsWithSegments("/api") &&
                            ctx.Response.StatusCode == 200)
                            ctx.Response.StatusCode = 401;
                        else
                            ctx.Response.Redirect(ctx.RedirectUri);
                        await Task.Yield(); // Create async awaitable task 
                    }
                };
            });

            // Configure services 

            //Node service for file generation
            services.AddNodeServices();

            // Add repositories 
            SetRepositoryDependencies(services);

            // Add fitness services 
            SetServiceDependencies(services);

            // MVC 
            services.AddLogging();
            services.AddMvc();

            // Add claim based authorization 
            services.AddAuthorization(options =>
                {
                    options.AddPolicy("AccountAccessPolicy",
                        policyBuilder => policyBuilder.RequireClaim(ClaimType.AccountAccess.ToString(), "Enabled"));
                    options.AddPolicy("WorkoutAccessPolicy",
                        policyBuilder => policyBuilder.RequireClaim(ClaimType.WorkoutAccess.ToString(), "Enabled"));
                    options.AddPolicy("ChallengeAccessPolicy",
                        policyBuilder => policyBuilder.RequireClaim(ClaimType.ChallengeAccess.ToString(), "Enabled"));
                    options.AddPolicy("ShareAccessPolicy",
                        policyBuilder => policyBuilder.RequireClaim(ClaimType.ShareAccess.ToString(), "Enabled"));
                }
            );

            // Require SSL for safe encrypted data transfer between client and app 
            services.Configure<MvcOptions>(options =>
            {
                // If production, setup HTTPS to receive client requests 
                if (Environment.IsProduction())
                    options.Filters.Add(new RequireHttpsAttribute());
            });

            // Use default camel case JSON data 
            services.Configure<MvcJsonOptions>(
                options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            // Add Auto Mapper to map between DTOs across app layers 
            services.AddAutoMapper();

            // Configure Email provider 
            services.Configure<AuthMessageSenderOptions>(Configuration);
        }

        /// <summary>
        ///     Dependency setup of repositories
        /// </summary>
        /// <param name="services"></param>
        private void SetRepositoryDependencies(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWorkoutRepository, WorkoutRepository>();
            services.AddScoped<IExerciseRepository, ExerciseRepository>();
            services.AddScoped<IWorkoutEntryRepository, WorkoutEntryRepository>();
            services.AddScoped<IRankingRepository, RankingRepository>();
            services.AddScoped<IChallengeRepository, ChallengeRepository>();
        }

        /// <summary>
        ///     Set dependencies for services
        /// </summary>
        /// <param name="services"></param>
        private void SetServiceDependencies(IServiceCollection services)
        {
            services.AddScoped<IFIleProvider<PortableUserDto>, PdfProvider>();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWorkoutService, WorkoutService>();
            services.AddScoped<IExerciseService, ExerciseService>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<IChallengeService, ChallengeService>();
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        ///     This allows the below serviced to be injected where used throughout the application
        ///     Dependency injection used to setup container for different services required by APP
        /// </summary>
        /// <param name="app"> Configure app request pipeline. </param>
        /// <param name="env"> Provide information about hosting environment. </param>
        /// <param name="loggerFactory"> Represents type used to configure logging system. </param>
        /// <param name="context"> Database context used to represent data model. </param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            Context context)
        {
            // Configure data mapping profiles 
            Mapper.Initialize(config =>
            {
                config.AddProfile<UserProfile>();
                config.AddProfile<WorkoutProfile>();
                config.AddProfile<ExerciseProfile>();
                config.AddProfile<ChallengeProfile>();
            });

            // Configure logging 
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Redirect all HTTP requests to HTTPS for safe communication 
            var options = new RewriteOptions().AddRedirectToHttps();
            app.UseRewriter(options);

            // Configure environment configuration 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Add Identity
            app.UseIdentity();

            // Add external (e.g. Facebook) middleware
            app.UseFacebookAuthentication(new FacebookOptions
            {
                AppId = Configuration["Authentication:Facebook:AppId"],
                AppSecret = Configuration["Authentication:Facebook:AppSecret"]
            });

            //Add sessions
            app.UseSession();


            // Check if db exists or create 
            DbInitializer.Initialize(context);

            // Configure MVC 
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "Default",
                    "{controller}/{action}/{id?}",
                    new {controller = "Home", action = "Index"});
            });

        }
    }
}