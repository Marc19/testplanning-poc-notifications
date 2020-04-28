using System;
namespace NotificationServer.Events
{
    public class FailedEvent : Event
    {
        public readonly string FailureReason;
    
        public FailedEvent(string failureReason, long loggedInUserId, Guid sagaId)
            : base(loggedInUserId, sagaId)
        {
            FailureReason = failureReason;
        }
    }
}
