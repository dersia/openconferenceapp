using open.conference.app.server.backend.DataObjects;
using open.conference.app.server.backend.Helpers;
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
    public class SessionController : BaseTableController<Session>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DevopenspaceContext context = new DevopenspaceContext();
            DomainManager = new EntityDomainManager<Session>(context, Request, true);
        }

        [QueryableExpand("Room,Speakers,MainCategory")]
        [EnableQuery(MaxTop = 500)]
        public IQueryable<Session> GetAllSession()
        {
            return Query();
        }

        [QueryableExpand("Speakers,Room,MainCategory")]
        public SingleResult<Session> GetSession(string id)
        {
            return Lookup(id);
        }

        [EmployeeAuthorize]
        public Task<Session> PatchSession(string id, Delta<Session> patch)
        {
            return UpdateAsync(id, patch);
        }

        [EmployeeAuthorize]
        public async Task<IHttpActionResult> PostSession(Session item)
        {
            Session current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [EmployeeAuthorize]
        [HttpPut]
        [ResponseType(typeof(List<Session>))]
        public async Task<IHttpActionResult> PutNotification(List<Session> items)
        {
            await Clean();
            var newItems = new List<Session>();
            foreach (var item in items)
            {
                newItems.Add(await InsertAsync(item));
            }
            return Ok(newItems);
        }

        [EmployeeAuthorize]
        public Task DeleteSession(string id)
        {
            return DeleteAsync(id);
        }

    }
}