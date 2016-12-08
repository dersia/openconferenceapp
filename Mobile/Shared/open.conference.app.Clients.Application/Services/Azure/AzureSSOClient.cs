using Microsoft.WindowsAzure.MobileServices;
using open.conference.app.Clients.ViewModels.Interfaces;
using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.DataObjects;
using open.conference.app.DataStore.Abstractions.Helpers;
using open.conference.app.DataStore.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.Clients.Application.Services.Azure
{
    public sealed class AzureSSOClient : ISSOClient
    {
        private readonly IStoreManager _storeManager;

        public AzureSSOClient(IStoreManager storeManager)
        {
            _storeManager = storeManager;
        }

        public async Task<AccountResponse> LoginAsync(string email = null)
        {
            MobileServiceUser user = await _storeManager.LoginAsync();

            return AccountFromMobileServiceUser(user, email);
        }

        public async Task LogoutAsync()
        {
            await _storeManager.LogoutAsync();
        }

        private AccountResponse AccountFromMobileServiceUser(MobileServiceUser user, string email = null)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IDictionary<string, string> claims = JwtUtility.GetClaims(user.MobileServiceAuthenticationToken);
            if(!claims.ContainsKey(JwtClaimNames.Subject) && !string.IsNullOrWhiteSpace(email))
            {
                claims.Add(JwtClaimNames.Subject, email);
            }

            var account = new AccountResponse();
            account.Success = true;
            account.User = new User(claims);

            return account;
        }
    }
}
