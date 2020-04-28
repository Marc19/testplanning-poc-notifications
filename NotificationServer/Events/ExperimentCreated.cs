using System;
namespace NotificationServer.Events
{
    public class ExperimentCreated : Event
    {
        public readonly long Id;

        public readonly string Creator;

        public readonly string Name;

        public readonly DateTime CreationDate;

        public ExperimentCreated(long id, string creator, string name, DateTime creationDate, long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            Id = id;
            Creator = creator;
            Name = name;
            CreationDate = creationDate;
        }
    }

    public class ExperimentCreationFailed : FailedEvent
    {
        public ExperimentCreationFailed(string failureReason, long loggedInUserId, Guid sagaId = default)
            : base(failureReason, loggedInUserId, sagaId) { }
    }
}
