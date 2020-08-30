using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using EventBusRabbitMQ;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using Ordering.API.Extensions;
using Ordering.API.RabbitMQ;
using Ordering.Application.Handlers;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Repositories;
using Ordering.Infrastructure.Repositories.Base;
using RabbitMQ.Client;

namespace Ordering.API
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
            services.AddControllers();

            //registering OrderContext for data access via Core Layer
            var conStr = Configuration.GetConnectionString("OrderConnection");
            services.AddDbContext<OrderContext>(options => options.UseSqlServer(conStr),ServiceLifetime.Singleton);

            //since at many places DI of IOrderRepository (OrderRepository) is required...
            //services.AddTransient<IOrderRepository, OrderRepository>();

            //since IOrdeRepository and OrderRepository depends on other intarfaces and classes
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IOrderRepository, OrderRepository>();
            //services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));



            //need DI of Automapper as the same is being used inside referenced assembly Ordering.Application
            services.AddAutoMapper(typeof(Startup));

            //need DI of MediatR as the same is being used inside referenced assembly Ordering.Application, as well as in OrderController
            services.AddMediatR(typeof(CheckoutOrderHandler).GetTypeInfo().Assembly);

            //for RabbitMQ connection
            services.AddSingleton<IRabbitMQConnection>(sp =>
            {
                var factory = new ConnectionFactory();

                if (!string.IsNullOrEmpty(Configuration["EventBus:HostName"]))
                    factory.HostName = Configuration["EventBus:HostName"];
                if (!string.IsNullOrEmpty(Configuration["EventBus:UserName"]))
                    factory.UserName = Configuration["EventBus:UserName"];
                if (!string.IsNullOrEmpty(Configuration["EventBus:Password"]))
                    factory.Password = Configuration["EventBus:Password"];

                return new RabbitMQConnection(factory);
            });
            services.AddSingleton<EventBusRabbitMQConsumer>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order API", Version = "v1" });
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseRabbitListener();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API V1");
            });
        }
    }
}
