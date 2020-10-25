using System;
using Asp_Net_Core_Masterclass.Models;
using Asp_Net_Core_Masterclass.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asp_Net_Core_Masterclass
{
    public class Startup
    {
    //we create a private field "_config"
        private IConfiguration _config;

        //we create a constructor and pass a parameter "config" of type "IConfiguration"
        public Startup(IConfiguration config)
        {
            //we then assign the private field to our parameter
            _config = config;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Ds service is for connecting to our database using our dbContext class and sqlServer.
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("DefaultConnection")));
            //Init our created DbContext() class via DI
            services.AddDbContext<AppDbContext>(options =>
            {
                //Here we indicate to use sqlLite and we pass the connection string dt it will use to connect to our database
                //With our _config dt contains the app.settngs.json and appsettings.Development.json, we call the GetconnectionString method dt looks inside d dev file and retreive a key with the value "DefaultConnection"
            });

            //here we configure our IdentityUser and specify our password complexity to override the default.
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    // Default Password settings.
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 9;
                    options.Password.RequiredUniqueChars = 0;
                    //here we add support for Email to be confirmed before user can login
                    options.SignIn.RequireConfirmedEmail = true;
                    //here we add support for our custom email token provider.
                    options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
                    //After 5 invalid password, acct is locked for 15mins
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                }).AddEntityFrameworkStores<AppDbContext>()
                //here we add token provider that will be used to generate email confirmation token
                .AddDefaultTokenProviders()

                    //We register our custom token provider
                .AddTokenProvider<CustomEmailConfirmationTokenProvider
                <ApplicationUser>>("CustomEmailConfirmation");
            // Changes token lifespan of all token types to 5hrs
            services.Configure<DataProtectionTokenProviderOptions>(o =>
                o.TokenLifespan = TimeSpan.FromHours(5));

            // Changes token lifespan of just the Email Confirmation Token type to 3days
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o =>
                o.TokenLifespan = TimeSpan.FromDays(3));

            //d first service our app depends on is ds which allows us to use d Mvc pattern./// we also configured Authorization
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();

            //Adding Google OAuth
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "366523375037-uuf4tfij2se3kbjqtvq5k8kpe1j6vf1q.apps.googleusercontent.com";
                options.ClientSecret = "3PxwvwfJLkdnMvygUv66AIyr";
            })
            //Adding Facebook OAuth
            .AddFacebook(options =>
            {
                options.AppId = "538903050053766";
                options.AppSecret = "fe1ccbb031d8dc6b236e7391b7c8a7ba";
            });

            //here we change the default AccessDenied route
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });


            //Here we provide our custom authorization requirement to the AddAuthorization() method.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role", "true"));
                options.AddPolicy("EditRolePolicy", policy =>
                    policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
                //if the first handler return failure, the next handler will not be called
               // options.InvokeHandlersAfterFailure = false;
            });


            //We register our first handler CanEditOnlyOtherAdminRolesAndClaimsHandler
            services.AddSingleton<IAuthorizationHandler,
                CanEditOnlyOtherAdminRolesAndClaimsHandler>();

            //We register our second handler SuperAdminHandler.cs
            services.AddSingleton<IAuthorizationHandler,
                SuperAdminHandler>();

            //we register our purpose string class which we will use to encrypt and decrypt values like route id.
            services.AddSingleton<DataProtectionPurposeStrings>();


            /*bool AuthorizeAccess(AuthorizationHandlerContext context)
            {
                return context.User.IsInRole("Admin") &&
                       context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                       context.User.IsInRole("Super Admin");
            }*/


            //d second service our app depends on is our IEmployeeRepository so we add it here using the "AddScoped()" method.. ds methods takes the service name and "optionally" takes the implementation class name (if we have the implementation in a separate class)
          services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>(); //we tell the app to use our "SQLEmployeeRepository" implementation
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //If we are on dev environment, we use the default Development exception page
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //if we are running on staging or production, we use the UseStatusCodePages() middleware.
            else
            {
                //Ds globally handle exceptions dt are thrown from the code dt are not from http request
                app.UseExceptionHandler("/Error");
                //We pass the url we want to go to if we have an error : /Error/{0}" will return "localhost:50001/Error/404" if we have 404 error code. And this will be managed by the "Error Controller"
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseStaticFiles();
            //we add our authentication middleware above "useMVC()|" middleware so dt users can be authenticated before they get to MVC
            app.UseAuthentication();
            //app.UseMvcWithDefaultRoute();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
