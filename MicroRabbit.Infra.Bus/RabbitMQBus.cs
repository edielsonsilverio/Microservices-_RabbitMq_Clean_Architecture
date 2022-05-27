using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MicroRabbit.Infra.Bus
{
    public sealed class RabbitMQBus : IEventBus
    {
        private readonly IMediator _mediator;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RabbitMQSettings _settings;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;

        public RabbitMQBus(
            IMediator mediator, 
            IOptions<RabbitMQSettings> settings, 
            IServiceScopeFactory serviceScopeFactory )
        {
            _mediator = mediator;
            _settings = settings.Value;
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new List<Type>();
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Publish<T>(T @event) where T : Event
        {
            //Configura a conexão
            var factory = GetConnection();

            using (var connection = factory.CreateConnection()) //Cria a Conexão
            using (var channel = connection.CreateModel())      //Cria um canal a partir da conexão.
            {
                var eventName = @event.GetType().Name;          //Obtêm o nome do objeto instânciado

                channel.QueueDeclare(eventName, false, false, false, null);

                var message = JsonConvert.SerializeObject(@event);

                var body = Encoding.UTF8.GetBytes(message);

                //Parametros
                //  1 - Exchange => ""
                //  2 - RouteKey => eventName (nome de indetificação da fila)
                //  3 - BaseProperties => null
                //  4 - Body => body (mensagem que será enviada)
                channel.BasicPublish("", eventName, null, body);
            }
        }

        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name; // Nome do evento
            var handlerType = typeof(TH);   // Nome do manipulador

            //Verefica se o evento não existe, então adiciona
            if (!_eventTypes.Contains(typeof(T)))
                _eventTypes.Add(typeof(T));

            //Verifica se a chave não existe e adiciona na lista.
            if (!_handlers.ContainsKey(eventName))
                _handlers.Add(eventName, new List<Type>());

            //Se handler com o nome do evento já existe, então lança uma exceção.
            if (_handlers[eventName].Any(s => s.GetType() == handlerType))
                throw new ArgumentException($"O handler exception {handlerType.Name} já foi registrado anteriormente pro '{eventName}'", nameof(handlerType));

            //Adiciona o evento na lista
            _handlers[eventName].Add(handlerType);

            //Método para consumir os eventos
            StartBasicConsume<T>();
        }

        private void StartBasicConsume<T>() where T : Event
        {
            var factory = GetConnection(dispatchConsumersAsync: true);

            var connection = factory.CreateConnection();//Cria a Conexão
            var channel = connection.CreateModel();     //Cria um canal a partir da conexão.

            var eventName = typeof(T).Name;

            channel.QueueDeclare(eventName, false, false, false, null);

            //Cria um evento para consumir as mensagens do canal
            var consumer = new AsyncEventingBasicConsumer(channel);

            //Dispara o evento
            consumer.Received += Consumer_Received;

            //Consome a mensagem e tira da fila.
            channel.BasicConsume(eventName, true, consumer);
        }

        private ConnectionFactory GetConnection(bool dispatchConsumersAsync = false)
        {
            //Configura a conexão
            var factory = new ConnectionFactory
            {
                HostName = _settings.Hostname,
                UserName = _settings.Username,
                Password = _settings.Password,
                DispatchConsumersAsync = dispatchConsumersAsync  //Informa que o consumer será asincrono.
            };
            return factory;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;                       // Obtêm o nome do evento já configurado na fila
            var message = Encoding.UTF8.GetString(e.Body.Span); // Obtêm a mensagem

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {

            }

        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_handlers.ContainsKey(eventName))
            {
                using(var scope = _serviceScopeFactory.CreateScope())
                {
                    var subscritpions = _handlers[eventName];

                    foreach (var subscritpion in subscritpions)
                    {
                        //var handler = Activator.CreateInstance(subscritpion); // Cria uma instância do subscritpion
                        var handler = scope.ServiceProvider.GetService(subscritpion);
                        if (handler == null) continue;

                        var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                        var @event = JsonConvert.DeserializeObject(message, eventType);

                        //Cria uma instância concreta do objeto genérico
                        var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                        //Executa um método a partir da instância criada.
                        var nameMethod = "Handle";
                        await (Task)concreteType.GetMethod(nameMethod).Invoke(handler, new object[] { @event });
                    }
                }
            }
        }
    }
}