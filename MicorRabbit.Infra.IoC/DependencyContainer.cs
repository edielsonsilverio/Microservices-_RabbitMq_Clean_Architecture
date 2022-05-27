using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace MicorRabbit.Infra.IoC
{
    public static class DependencyContainer
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services,IConfiguration configuration)
        {

            //MediatR Mediator
            services.AddMediatR(Assembly.GetExecutingAssembly());

            //Domain Bus
            services.AddSingleton<IEventBus, RabbitMQBus>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var optionFactory = sp.GetService<IOptions<RabbitMQSettings>>();
                return new RabbitMQBus(sp.GetService<IMediator>(), optionFactory, scopeFactory);
            });

            //Application Services
            //services.AddTransient<IRequestHandler<CreateTransferCommand,bool>,TransferCommandHandler>();
            //services.AddTransient<IAccountService, AccountService>();
            //services.AddTransient<IAccountRespository, AccountRespository>();
            //services.AddTransient<BankingDbContext>();

            //Data
            //services.AddTransient<ITransferService, TransferService>();
            //services.AddTransient<ITransferRepository, TransferRepository>();
            //services.AddTransient<TransferDbContext>();


            return services;
        }
    }
}
