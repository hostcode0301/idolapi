using System.Collections.Generic;
using System.Threading.Tasks;
using idolapi.DB.DTOs;

namespace idolapi.Hubs
{
    /// <summary>
    /// Self define send method
    /// </summary>
    public interface INotifyClient
    {
        Task ReceiveMessage(NotifyMessage message);
        // Task ReceiveHistory(T);
    }
}