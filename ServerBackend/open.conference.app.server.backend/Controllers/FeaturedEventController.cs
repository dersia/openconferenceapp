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
    public class FeaturedEventController : BaseTableController<FeaturedEvent>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DevopenspaceContext context = new DevopenspaceContext();
            DomainManager = new EntityDomainManager<FeaturedEvent>(context, Request, true);
        }

        [QueryableExpand("Sponsor")]
        public IQueryable<FeaturedEvent> GetAllFeaturedEvent()
        {
            return Query();
        }

        [QueryableExpand("Sponsor")]
        public SingleResult<FeaturedEvent> GetFeaturedEvent(string id)
        {
            return Lookup(id);
        }

        [EmployeeAuthorize]
        public Task<FeaturedEvent> PatchFeaturedEvent(string id, Delta<FeaturedEvent> patch)
        {
            return UpdateAsync(id, patch);
        }

        [EmployeeAuthorize]
        public async Task<IHttpActionResult> PostFeaturedEvent(FeaturedEvent item)
        {
            FeaturedEvent current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [EmployeeAuthorize]
        [HttpPut]
        [ResponseType(typeof(List<FeaturedEvent>))]
        public async Task<IHttpActionResult> PutNotification(List<FeaturedEvent> items)
        {
            await Clean();
            var newItems = new List<FeaturedEvent>();
            foreach (var item in items)
            {
                newItems.Add(await InsertAsync(item));
            }
            return Ok(newItems);
        }

        [EmployeeAuthorize]
        public Task DeleteFeaturedEvent(string id)
        {
            return DeleteAsync(id);
        }
    }
}