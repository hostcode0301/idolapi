using System.Collections.Generic;
using System.Threading.Tasks;
using idolapi.DB.DTOs;
using idolapi.Services;
using Microsoft.AspNetCore.SignalR;

namespace idolapi.Hubs
{
    public class NotifyHub : Hub
    {
        private readonly ICacheProvider _cacheProvider;
        // Static List to do not renew a list every time new Hub created
        private static List<NotifyMessage> history = new List<NotifyMessage>();
        // Name of key in Redis storage
        private string HISTORY = "history";

        public NotifyHub(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        /// <summary>
        /// When a new people connect to Notification call GetHistory
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await GetHistory();

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Get list of history from chat then append it to static list
        /// </summary>
        private async Task GetHistory()
        {
            // Check only update history when first time load the hub
            if (history.Count == 0)
            {
                // Try to get history from db
                var oldHistory = await _cacheProvider.GetFromCache<List<NotifyMessage>>(HISTORY);

                // Prevent null oldHistory be assign to history list
                if (oldHistory != null)
                {
                    history.AddRange(oldHistory);
                }
            }

            // Send history list to client
            await Clients.Caller.SendAsync("ReceiveHistory", history);
        }

        /// <summary>
        /// Every time client chat a new message
        /// </summary>
        /// <param name="message">Message from client</param>
        public async Task SendNotification(NotifyMessage message)
        {
            // Add new message to history list
            history.Add(message);

            // Add a list to cache
            await _cacheProvider.SetCache<List<NotifyMessage>>(HISTORY, history);

            // Send new message to all other client
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}