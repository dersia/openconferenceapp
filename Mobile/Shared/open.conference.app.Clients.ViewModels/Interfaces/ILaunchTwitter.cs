using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.Clients.ViewModels.Interfaces
{
    public interface ILaunchTwitter
    {
        Task<bool> OpenUserName(string username);
        Task<bool> OpenStatus(string statusId);
    }
}
