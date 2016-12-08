using open.conference.app.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.Clients.ViewModels.Interfaces
{
    public interface ISSOClient
    {
        Task<AccountResponse> LoginAsync(string email = null);

        Task LogoutAsync();
    }
}
