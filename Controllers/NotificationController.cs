using System.Threading.Tasks;
using idolapi.DB.DTOs;
using idolapi.Hubs;
using idolapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace idolapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<NotifyHub> _notifyHub;
        private readonly ICacheProvider _cacheProvider;

        public NotificationController(IHubContext<NotifyHub> notifyHub, ICacheProvider cacheProvider)
        {
            _notifyHub = notifyHub;
            _cacheProvider = cacheProvider;
        }

        [HttpPost("notify")]
        public async Task Notify(NotifyMessage notifyMessage)
        {
            await _cacheProvider.SetCache<NotifyMessage>("", notifyMessage);
            await _notifyHub.Clients.All.SendAsync("ReceiveMessage", notifyMessage);
        }
    }
}