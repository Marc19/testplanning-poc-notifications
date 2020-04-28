using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NotificationServer.Hubs
{
    public class NotificationsHub : Hub
    {
        /*
         * Now I know how to send to a specific connection Id
         * The idea is, in the frontend, once connected, I have the connection Id
         * Now any request sent to backend, bears this connection Id
         * Any IMessage in backend should have a connectionId
         * So the notification server will listen to kafka events
         * Once it has an event, it will send message to connectionId accordingly
         *
         * There is another solution, but it must have a logged in user
         * We will use the user id then as a unique group name to be created in SignalR Hub
         * Now once the user is connected, he will be added to the group with the name of his id
         * So the notification server will listen to kafka events
         * Once it has an event, it will send message to group name accordingly
         *
         * These 2 solutions assume that SendMessage can be called from server normally
         * when a message is consumed by Kafka.
         */

        public NotificationsHub()
        {
            Console.Write("Test");
        }

        public override Task OnConnectedAsync()
        {
            long loggedInUserId = GetLoggedInUserIdMockUp();
            Groups.AddToGroupAsync(Context.ConnectionId, loggedInUserId.ToString());
            return base.OnConnectedAsync();
        }

        public async Task SendMessage(long loggedInUserId, string message)
        {
            message = "ProcessedMessage";

            var x = new { myKey = message };

            await Clients.Groups(loggedInUserId.ToString()).SendAsync("ReceiveMessage", "static", x);
            //await Clients.All.SendAsync("ReceiveMessage", user, x);
        }

        private long GetLoggedInUserIdMockUp()
        {
            string jwtInput = Context.GetHttpContext().Request.Query["token"];

            var jwtHandler = new JwtSecurityTokenHandler();

            if (!jwtHandler.CanReadToken(jwtInput)) throw new Exception("The token doesn't seem to be in a proper JWT format.");

            var token = jwtHandler.ReadJwtToken(jwtInput);

            var jwtPayload = JsonConvert.SerializeObject(token.Claims.Select(c => new { c.Type, c.Value }));

            JArray rss = JArray.Parse(jwtPayload);
            var firstChild = rss.First;
            var lastChild = firstChild.Last;
            var idString = lastChild.Last.ToString();

            long.TryParse(idString, out long id);

            return id;
        }
    }
}
