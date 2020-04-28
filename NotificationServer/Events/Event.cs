using System;
namespace NotificationServer.Events
{
    public class Event
    {
        public readonly long LoggedInUserId;

        public Guid SagaId { get; set; }

        public Event(long loggedInUserId, Guid sagaId)
        {
            LoggedInUserId = loggedInUserId;
            SagaId = sagaId;
        }
    }
}
