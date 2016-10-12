using developer.open.space.server.backend.DataObjects;
using developer.open.space.server.backend.Helpers;
using developer.open.space.server.backend.Identity;
using developer.open.space.server.backend.Models;
using Microsoft.Azure.Mobile.Server;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.OData;

namespace developer.open.space.server.backend.Controllers
{
    public class WorkshopController : BaseTableController<Workshop>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DevopenspaceContext context = new DevopenspaceContext();
            DomainManager = new EntityDomainManager<Workshop>(context, Request, true);
        }

        [QueryableExpand("Room,Speakers,MainCategory")]
        [EnableQuery(MaxTop = 500)]
        public IQueryable<Workshop> GetAllWorkshop()
        {
            return Query();
        }

        [QueryableExpand("Speakers,Room,MainCategory")]
        public SingleResult<Workshop> GetWorkshop(string id)
        {
            return Lookup(id);
        }

        [EmployeeAuthorize]
        public Task<Workshop> PatchWorkshop(string id, Delta<Workshop> patch)
        {
            return UpdateAsync(id, patch);
        }

        [EmployeeAuthorize]
        public async Task<IHttpActionResult> PostWorkshop(Workshop item)
        {
            Workshop current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [EmployeeAuthorize]
        [HttpPut]
        [ResponseType(typeof(List<Workshop>))]
        public async Task<IHttpActionResult> PutNotification(List<Workshop> items)
        {
            await Clean();
            var newItems = new List<Workshop>();
            foreach (var item in items)
            {
                newItems.Add(await InsertAsync(item));
            }
            return Ok(newItems);
        }

        [EmployeeAuthorize]
        public Task DeleteWorkshop(string id)
        {
            return DeleteAsync(id);
        }

    }
}