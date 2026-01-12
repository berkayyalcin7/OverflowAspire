using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Retry;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Common
{
    public static class WolverineExtensions
    {
        public static async Task UseWolverineWithRabbitMqAsync(this IHostApplicationBuilder builder,IConfiguration config,Action<WolverineOptions> configureMessaging)
        {
            // RabbitMQ Connection Retry Policy
            var retryPolicy = Policy.Handle<BrokerUnreachableException>().Or<SocketException>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount) =>
                {
                    Console.WriteLine($"Retry attemp {retryCount} failed . Retrying in {timeSpan.Seconds} seconds");
                });

            await retryPolicy.ExecuteAsync(async () =>
            {
                var endpoint = builder.Configuration.GetConnectionString("messaging") ?? throw new InvalidOperationException("Messaging connection string not found");

                var factory = new RabbitMQ.Client.ConnectionFactory()
                {
                    Uri = new Uri(endpoint)
                };

                await using var connection = await factory.CreateConnectionAsync();

            });

            // OpenTelemetry Tracing
            builder.Services.AddOpenTelemetry().WithTracing(trace =>
            {
                trace.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(builder.Environment.ApplicationName))
                    .AddSource("Wolverine");
            });

            builder.UseWolverine(opts =>
            {
                opts.UseRabbitMqUsingNamedConnection("messaging").AutoProvision()
                .DeclareExchange("questions");
                //opts.PublishAllMessages().ToRabbitExchange("questions");

                configureMessaging(opts);
            });


        }
    }
}
