using System;
namespace NotificationServer.Events
{
    public class ExperimentDeleted : Event
    {
        public readonly long Id;

        public ExperimentDeleted(long id, long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            Id = id;
        }
    }

    public class ExperimentDeletionFailed : FailedEvent
    {
        public ExperimentDeletionFailed(string failureReason, long loggedInUserId, Guid sagaId = default)
            : base(failureReason, loggedInUserId, sagaId) { }
    }
}
