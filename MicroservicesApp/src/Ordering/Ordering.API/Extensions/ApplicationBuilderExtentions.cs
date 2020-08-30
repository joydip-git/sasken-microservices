using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.API.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Extensions
{
    public static class ApplicationBuilderExtentions
    {
        public static EventBusRabbitMQConsumer Listener { get; set; }
        public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
        {
            try
            {
                Listener = app.ApplicationServices.GetService<EventBusRabbitMQConsumer>();
                var life = app.ApplicationServices.GetService<IHostApplicationLifetime>();
                life.ApplicationStarted.Register(() => Listener.Consume());
                life.ApplicationStopping.Register(() => Listener.Disconnect());
                return app;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
