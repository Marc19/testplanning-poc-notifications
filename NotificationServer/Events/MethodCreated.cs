using System;
namespace NotificationServer.Events
{
    public class MethodCreated : Event
    {
        public readonly long Id;

        public readonly string Creator;

        public readonly string Name;

        public readonly decimal ApplicationRate;

        public readonly DateTime CreationDate;

        public MethodCreated(long id, string creator, string name, decimal applicationRate,
            DateTime creationDate, long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            Id = id;
            Creator = creator;
            Name = name;
            ApplicationRate = applicationRate;
            CreationDate = creationDate;
        }
    }

    public class MethodCreationFailed : FailedEvent
    {
        public MethodCreationFailed(string failureReason, long loggedInUserId, Guid sagaId = default)
            : base(failureReason, loggedInUserId, sagaId) { }
    }
}
