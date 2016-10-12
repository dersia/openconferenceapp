using developer.open.space.DataStore.Abstractions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Abstractions.DataObjects
{
    public class User
    {
        private string _email = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _gravatarUrl = string.Empty;
        public User(IDictionary<string,string> userClaims)
        {
            UserClaims = userClaims;
            userClaims.TryGetValue(JwtClaimNames.Subject, out _email);
            userClaims.TryGetValue(JwtClaimNames.GivenName, out _firstName);
            userClaims.TryGetValue(JwtClaimNames.FamilyName, out _lastName);
            if(!string.IsNullOrWhiteSpace(_email))
            {
                _gravatarUrl = Gravatar.GetURL(_email);
            }
        }

        public IDictionary<string, string> UserClaims { get; }
        public string Email { get { return _email; } }
        public string FirstName { get { return _firstName; } }
        public string LastName { get { return _lastName; } }
        public string GravatarUrl { get { return _gravatarUrl; } }
    }

    public class AccountResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
    }

    [Flags]
    public enum Access
    {
        None = 0,
        Admin = 1,
        Write = 1 << 1,
        Read = 1 << 2,
    }
}
