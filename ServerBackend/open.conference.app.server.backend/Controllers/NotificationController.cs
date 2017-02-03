using open.conference.app.server.backend.DataObjects;
using open.conference.app.server.backend.Identity;
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
    public class NotificationController : BaseTableController<Notification>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DevopenspaceContext context = new DevopenspaceContext();
            DomainManager = new EntityDomainManager<Notification>(context, Request, true);
        }

        public IQueryable<Notification> GetAllNotification()
        {
            return Query();
        }

        public SingleResult<Notification> GetNotification(string id)
        {
            return Lookup(id);
        }

        [EmployeeAuthorize]
        public Task<Notification> PatchNotification(string id, Delta<Notification> patch)
        {
            return UpdateAsync(id, patch);
        }

        [EmployeeAuthorize]
        public async Task<IHttpActionResult> PostNotification(Notification item)
        {
            Notification current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [EmployeeAuthorize]
        [HttpPut]
        [ResponseType(typeof(List<Notification>))]
        public async Task<IHttpActionResult> PutNotification(List<Notification> items)
        {
            await Clean();
            var newItems = new List<Notification>();
            foreach (var item in items)
            {
                newItems.Add(await InsertAsync(item));
            }
            return Ok(newItems);
        }

        [EmployeeAuthorize]
        public Task DeleteNotification(string id)
        {
            return DeleteAsync(id);
        }
    }
}