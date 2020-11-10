using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Net.WebSockets;
using System.Threading;

using Kraken.Api.Sensor.Models;
using Kraken.Api.Sensor.Services;

namespace Kraken.Api.Sensor
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

        private Consumer _consumer;

        /// <summary>
        /// This method is called by the runtime and is used for setting up configuration, services,
        /// controllers and authentication.
        /// </summary>
        /// <param name="services">A collection of services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            AssertApiConfigured();

            services.Configure<ConnectionInfo>(Configuration.GetSection("DatabaseSettings"));
            services.AddSingleton<IConnectionInfo>(provider => provider.GetRequiredService<IOptions<ConnectionInfo>>().Value);

            services.AddSingleton<ApplicationService>();
            services.AddSingleton<DataService>();
            services.AddSingleton<WebSocketService>();

            services.AddControllers();

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
        /// Creates a new ActiveMQ consumer in order to listen to incoming sensor measurements.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        /// <param name="lifetime">Host application lifetime.</param>
        /// <param name="dataService">The data service.</param>
        /// <param name="webSocketService">The websocket service.</param>
        private void CreateQueueConsumer(IConfigurationSection configuration,
                                        IHostApplicationLifetime lifetime,
                                        DataService dataService,
                                        WebSocketService webSocketService)
        {
            _consumer = new Consumer(dataService, webSocketService);
            var thread = new Thread((Object stateInfo) => _consumer.Listen());

            string connectionString = configuration.GetValue<string>("ConnectionString");
            string queue = configuration.GetValue<string>("Name");

            // Run the consumers in a specific order, to ensure the consumer always disconnects nicely.
            try
            {
                lifetime.ApplicationStopping.Register(() => _consumer.Disconnect());
                _consumer.Connect(connectionString, queue);
                thread.Start();
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"Unable to start queue consumer: {exception.Message}");
                lifetime.StopApplication();
            }
        }

        /// <summary>
        /// This method is called by the runtime and is used to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">Provides information about the hosting environment the program is running in.</param>
        /// <param name="lifetime">The host application lifetime.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("DevelopmentPolicy");

            app.UseWebSockets();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            AcceptWebSocketRequests(app);

            IServiceProvider serviceProvider = app.ApplicationServices;
            CreateQueueConsumer(Configuration.GetSection("Queue"), lifetime,
                serviceProvider.GetService<DataService>(), serviceProvider.GetService<WebSocketService>());
        }

        /// <summary>
        /// Configures the API to accept incoming websocket requests.
        /// </summary>
        /// <param name="app">The application builder.</param>
        public void AcceptWebSocketRequests(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var taskCompletionSource = new TaskCompletionSource<object>();

                        app.ApplicationServices.GetService<WebSocketService>().AddClient(webSocket);

                        await taskCompletionSource.Task;

                        return;
                    }

                    context.Response.StatusCode = 400;

                    return;
                }

                await next();
            });
        }
    }
}
