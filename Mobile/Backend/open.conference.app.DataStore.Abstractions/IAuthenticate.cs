using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Abstractions
{
    public interface IAuthenticate
    {
        Task<MobileServiceUser> Authenticate();
    }
}
