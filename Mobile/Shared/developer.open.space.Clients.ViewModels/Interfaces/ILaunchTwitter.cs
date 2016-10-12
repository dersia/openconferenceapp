using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.Clients.ViewModels.Interfaces
{
    public interface ILaunchTwitter
    {
        Task<bool> OpenUserName(string username);
        Task<bool> OpenStatus(string statusId);
    }
}
