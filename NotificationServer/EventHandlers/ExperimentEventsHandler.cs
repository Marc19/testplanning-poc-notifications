using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using NotificationServer.Events;
using NotificationServer.Hubs;

namespace NotificationServer.EventHandlers
{
    public class ExperimentEventsHandler : IExperimentEventsHandler
    {
        private readonly IHubContext<NotificationsHub> _hubContext;

        public ExperimentEventsHandler(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void Handle(Event @event)
        {
            switch (@event)
            {
                case ExperimentCreated ec: HandleExperimentCreated(ec); break;
                case ExperimentCreationFailed ecf: HandleExperimentCreationFailed(ecf); break;
                case ExperimentDeleted ed: HandleExperimentDeleted(ed); break;
                case ExperimentDeletionFailed edf: HandleExperimentDeletionFailed(edf); break;
                case ExperimentUpdated eu: HandleExperimentUpdated(eu); break;
                case ExperimentUpdateFailed euf: HandleExperimentUpdateFailed(euf); break;
                case ExperimentCreatedWithMethods em: HandledExperimentCreatedWithMethods(em); break;
                case ExperimentWithMethodsCreationFailed m: HandleExperimentWithMethodsCreationFailed(m); break;
                case MethodCreated m: HandleMethodCreated(m); break;
                case MethodCreationFailed m: HandleMethodCreationFailed(m); break;
            }
        }

        private async void HandleMethodCreationFailed(MethodCreationFailed m)
        {
            var failedMethod = new { failureReason = m.FailureReason };
            await _hubContext.Clients.Groups(m.LoggedInUserId.ToString()).SendAsync("ReceiveMethodCreatedMessage", failedMethod);
        }

        private async void HandleMethodCreated(MethodCreated m)
        {
            if (m.SagaId != default)
            {
                return;
            }

            var createdMethod = new
            {
                id = m.Id, creator = m.Creator, name = m.Name,
                applicationRate = m.ApplicationRate, creationDate = m.CreationDate
            };
            await _hubContext.Clients.Groups(m.LoggedInUserId.ToString()).SendAsync("ReceiveMethodCreatedMessage", createdMethod);
        }

        private async void HandledExperimentCreatedWithMethods(ExperimentCreatedWithMethods m)
        {
            ExperimentEventData experiment = m.Experiment;
            List<MethodEventData> methods = m.Methods;

            var mappedExperiment = new
            {
                id = experiment.Id,
                creator = experiment.Creator,
                name = experiment.Name,
                creationDate = experiment.CreationDate
            };

            var mappedMethods = methods.Select(method => new
            {
                id = method.Id,
                creator = method.Creator,
                name = method.Name,
                applicationRate = method.ApplicationRate,
                creationDate = method.CreationDate
            });

            var createdExperimentWithMethod = new
            {
                experiment = mappedExperiment,
                methods = mappedMethods
            };

            await _hubContext.Clients.Groups(m.LoggedInUserId.ToString()).SendAsync("ReceiveExperimentWithMethodsCreatedMessage", createdExperimentWithMethod);
        }

        private async void HandleExperimentWithMethodsCreationFailed(ExperimentWithMethodsCreationFailed m)
        {
            var failedMethod = new { failureReason = m.FailureReason };
            await _hubContext.Clients.Groups(m.LoggedInUserId.ToString()).SendAsync("ReceiveExperimentWithMethodsCreatedMessage", failedMethod);

        }

        private async void HandleExperimentCreated(ExperimentCreated c)
        {
            if(c.SagaId != default)
            {
                return;
            }

            var createdExperiment = new { id= c.Id, creator = c.Creator, name = c.Name, creationDate = c.CreationDate };
            await _hubContext.Clients.Groups(c.LoggedInUserId.ToString()).SendAsync("ReceiveExperimentCreatedMessage", createdExperiment);
        }

        private async void HandleExperimentCreationFailed(ExperimentCreationFailed cf)
        {
            var failedExperiment = new { failureReason = cf.FailureReason };
            await _hubContext.Clients.Groups(cf.LoggedInUserId.ToString()).SendAsync("ReceiveExperimentCreatedMessage", failedExperiment);
        }

        private void HandleExperimentDeleted(ExperimentDeleted u)
        {
            //todo
        }

        private void HandleExperimentDeletionFailed(ExperimentDeletionFailed c)
        {
            //todo
        }

        private void HandleExperimentUpdated(ExperimentUpdated d)
        {
            //todo
        }

        private void HandleExperimentUpdateFailed(ExperimentUpdateFailed u)
        {
            //todo
        }

    }
}
