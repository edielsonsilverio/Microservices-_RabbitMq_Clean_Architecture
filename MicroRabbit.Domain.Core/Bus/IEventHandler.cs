using MicroRabbit.Domain.Core.Events;
using System;

namespace MicroRabbit.Domain.Core.Bus
{
    public interface IEventHandler<in TEvent>:IEventHandler where TEvent : Event
    {
        Task Handle(TEvent @event);
    }

    public interface IEventHandler { }
}
