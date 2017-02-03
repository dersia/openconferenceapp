using open.conference.app.server.backend.DataObjects;
using open.conference.app.server.backend.Helpers;
using open.conference.app.server.backend.Models;
using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.OData;

namespace open.conference.app.server.backend.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FeedbackController : BaseTableController<Feedback>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DevopenspaceContext context = new DevopenspaceContext();
            DomainManager = new EntityDomainManager<Feedback>(context, Request, true);
        }

        public IQueryable<Feedback> GetAllFeedback()
        {
            var items = Query();

            var email = EmailHelper.GetAuthenticatedUserEmail(RequestContext);

            var final = items.Where(feedback => feedback.UserId == email);

            return final;

        }


        [Authorize]
        public SingleResult<Feedback> GetFeedback(string id)
        {
            return Lookup(id);
        }

        [Authorize]
        public Task<Feedback> PatchFeedback(string id, Delta<Feedback> patch)
        {
            return UpdateAsync(id, patch);
        }

        [Authorize]
        public async Task<IHttpActionResult> PostFeedback(Feedback item)
        {
            var feedback = item;
            feedback.UserId = EmailHelper.GetAuthenticatedUserEmail(RequestContext);

            var current = await InsertAsync(feedback);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }
    }
}