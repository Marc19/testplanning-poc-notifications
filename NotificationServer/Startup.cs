using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationServer.Consumers;
using NotificationServer.EventHandlers;
using NotificationServer.Hubs;

namespace NotificationServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        { 
            services.AddSignalR();

            services.AddTransient<IExperimentEventsHandler, ExperimentEventsHandler>();

            var consumerConfig = new ConsumerConfig();
            Configuration.Bind("consumer", consumerConfig);
            services.AddSingleton<ConsumerConfig>(consumerConfig);
            //Note :- Please make sure all the other related service(s) which you are using // part of your business logic are added here like below;    
            //services.AddTransient < interface.IMyBusinessServices, Implementations.MyBusinessServices > ();  
            services.AddHostedService<MyKafkaConsumer>(); //important

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .WithOrigins("http://localhost:3000", "http://localhost:3001")
                    .WithMethods("POST")
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("CorsPolicy");


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationsHub>("/notificationsHub");

                
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
            });
        }
    }
}
