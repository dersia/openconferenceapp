using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace developer.open.space.server.backend.Identity
{
    public class EmployeeAuthorizeAttribute : AuthorizeAttribute
    {

        private IList<string> employeeEmailAddresses = new List<string>
        {
        };
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);

            // If not already authenticated, return.
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var email = GetUserEmail(GetAccessToken(actionContext.Request.Headers)).Result;

            // If they don't have an identity name at all, return.
            if (string.IsNullOrEmpty(email))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }


            var address = IsValidEmail(email);

            // If their name is not a valid email, return.
            if (address == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            if (!employeeEmailAddresses.Contains(email.ToLower()))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

        }

        MailAddress IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr;
            }
            catch
            {
                return null;
            }
        }

        private string GetAccessToken(System.Net.Http.Headers.HttpRequestHeaders requestHeader)
        {
            return requestHeader?.GetValues("X-MS-TOKEN-MICROSOFTACCOUNT-ACCESS-TOKEN")?.FirstOrDefault();
        }

        private async Task<string> GetUserEmail(string accessToken)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = (await client.GetAsync("https://apis.live.net/v5.0/me" + "?access_token=" + accessToken).ConfigureAwait(false)))
                {
                    var o = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                    return o?["emails"]?["preferred"]?.ToString();
                }
            }
        }
    }
}