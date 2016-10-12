using developer.open.space.Clients.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using developer.open.space.DataStore.Abstractions.DataObjects;
using developer.open.space.DataStore.Abstractions.Helpers;

namespace developer.open.space.Clients.Application.Services
{
    public class MockSSOClient : ISSOClient
    {
        public async Task<AccountResponse> LoginAsync(string email = null)
        {
            return await Task.FromResult(new AccountResponse { Success = true, User = new User(new Dictionary<string, string> { [JwtClaimNames.Subject] = email }), Token = Guid.NewGuid().ToString() });
        }

        public async Task LogoutAsync()
        {
            await Task.FromResult(0);
        }
    }
}
