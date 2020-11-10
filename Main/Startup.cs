using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;

using Kraken.Api.Main.Models;
using Kraken.Api.Main.Services;
using Kraken.Api.Main.Services.Components;

namespace Kraken.Api.Main
{
    /// <summary>
    /// Invoked after the program starts and takes care of setting up services.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Creates a new instance of the Startup class.
        /// </summary>
        /// <param name="configuration">A key-value set of program properties.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <value>A key-value set of program properties, as defined in appsettings.json and appsettings.development.json.</value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method is called by the runtime and is used for setting up configuration, services,
        /// controllers and authentication.
        /// </summary>
        /// <param name="services">A collection of services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            AssertApiConfigured();

            services.Configure<DatabaseEnvironment>(Configuration.GetSection("DatabaseSettings"));
            services.AddSingleton<IDatabaseEnvironment>(provider => provider.GetRequiredService<IOptions<DatabaseEnvironment>>().Value);

            services.AddSingleton<AreaService>();
            services.AddSingleton<AuthService>();
            services.AddSingleton<Deferrer>();
            services.AddSingleton<HouseService>();
            services.AddSingleton<IntersectionService>();
            services.AddSingleton<PipeService>();
            services.AddSingleton<StreetService>();
            services.AddSingleton<SourceService>();
            services.AddSingleton<TokenService>();
            services.AddSingleton<UserService>();

            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Authentication:JwtIssuer"],
                        ValidAudience = Configuration["Authentication:JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:JwtKey"]))
                    };
                });

            services.AddCors(options => options.AddPolicy("DevelopmentPolicy",
                builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
        }

        /// <summary>
        /// Stops the program if it can be reasonably assumed that appsettings.json does not exist.
        /// We do this by checking if the 'DatabaseSettings' entry, which is of great importance, is
        /// present. This is not meant to combat malicious attempts to crash the program, it is simply
        /// meant as a way to notify developers if they have not run 'make config' yet.
        /// </summary>
        private void AssertApiConfigured()
        {
            if (!Configuration.GetSection("DatabaseSettings").Exists())
            {
                Console.Error.WriteLine("Unable to determine configuration. Did you run 'make config'?");
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// This method is called by the runtime and is used to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">Provides information about the hosting environment the program is running in.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("DevelopmentPolicy");

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
