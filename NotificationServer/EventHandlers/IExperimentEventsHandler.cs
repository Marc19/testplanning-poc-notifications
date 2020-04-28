using System;
using NotificationServer.Events;

namespace NotificationServer.EventHandlers
{
    public interface IExperimentEventsHandler
    {
        void Handle(Event @event);
    }
}
