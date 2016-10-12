using developer.open.space.server.backend.DataObjects;
using developer.open.space.server.backend.Identity;
using developer.open.space.server.backend.Models;
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

namespace developer.open.space.server.backend.Controllers
{
    public class SpeakerController : BaseTableController<Speaker>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DevopenspaceContext context = new DevopenspaceContext();
            DomainManager = new EntityDomainManager<Speaker>(context, Request, true);
        }

        public IQueryable<Speaker> GetAllSpeaker()
        {
            return Query();
        }

        public SingleResult<Speaker> GetSpeaker(string id)
        {
            return Lookup(id);
        }

        [EmployeeAuthorize]
        public Task<Speaker> PatchSpeaker(string id, Delta<Speaker> patch)
        {
            return UpdateAsync(id, patch);
        }

        [EmployeeAuthorize]
        [HttpPut]
        [ResponseType(typeof(List<Speaker>))]
        public async Task<IHttpActionResult> PutNotification(List<Speaker> items)
        {
            await Clean();
            var newItems = new List<Speaker>();
            foreach (var item in items)
            {
                newItems.Add(await InsertAsync(item));
            }
            return Ok(newItems);
        }

        [EmployeeAuthorize]
        public async Task<IHttpActionResult> PostSpeaker(Speaker item)
        {
            Speaker current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [EmployeeAuthorize]
        public Task DeleteSpeaker(string id)
        {
            return DeleteAsync(id);
        }

    }
}