using System;
using System.Collections.Generic;

namespace NotificationServer.Events
{
    public class ExperimentCreatedWithMethods : Event
    {
        public readonly ExperimentEventData Experiment;

        public readonly List<MethodEventData> Methods;

        public ExperimentCreatedWithMethods(ExperimentEventData experiment, List<MethodEventData> methods,
            long loggedInUserId, Guid sagaId = default)
            : base(loggedInUserId, sagaId)
        {
            Experiment = experiment;
            Methods = methods;
        }
    }

    public class ExperimentWithMethodsCreationFailed : FailedEvent
    {
        public ExperimentWithMethodsCreationFailed(string failureReason, long loggedInUserId, Guid sagaId = default)
            : base(failureReason, loggedInUserId, sagaId) { }
    }

    public class ExperimentEventData
    {
        public readonly long Id;

        public readonly string Creator;

        public readonly string Name;

        public readonly DateTime CreationDate;

        public ExperimentEventData(long id, string creator, string name, DateTime creationDate)
        {
            Id = id;
            Creator = creator;
            Name = name;
            CreationDate = creationDate;
        }
    }

    public class MethodEventData
    {
        public readonly long Id;

        public readonly string Creator;

        public readonly string Name;

        public readonly decimal ApplicationRate;

        public readonly DateTime CreationDate;

        public MethodEventData(long id, string creator, string name, decimal applicationRate, DateTime creationDate)
        {
            Id = id;
            Creator = creator;
            Name = name;
            ApplicationRate = applicationRate;
            CreationDate = creationDate;
        }
    }
}
