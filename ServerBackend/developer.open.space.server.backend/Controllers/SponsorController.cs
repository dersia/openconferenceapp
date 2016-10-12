using developer.open.space.server.backend.DataObjects;
using developer.open.space.server.backend.Helpers;
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
    public class SponsorController : BaseTableController<Sponsor>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DevopenspaceContext context = new DevopenspaceContext();
            DomainManager = new EntityDomainManager<Sponsor>(context, Request, true);
        }

        [QueryableExpand("SponsorLevel")]
        public IQueryable<Sponsor> GetAllSponsor()
        {
            return Query();
        }

        [QueryableExpand("SponsorLevel")]
        public SingleResult<Sponsor> GetSponsor(string id)
        {
            return Lookup(id);
        }

        [EmployeeAuthorize]
        public Task<Sponsor> PatchSponsor(string id, Delta<Sponsor> patch)
        {
            return UpdateAsync(id, patch);
        }

        [EmployeeAuthorize]
        public async Task<IHttpActionResult> PostSponsor(Sponsor item)
        {
            Sponsor current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [EmployeeAuthorize]
        [HttpPut]
        [ResponseType(typeof(List<Sponsor>))]
        public async Task<IHttpActionResult> PutNotification(List<Sponsor> items)
        {
            await Clean();
            var newItems = new List<Sponsor>();
            foreach (var item in items)
            {
                newItems.Add(await InsertAsync(item));
            }
            return Ok(newItems);
        }

        [EmployeeAuthorize]
        public Task DeleteSponsor(string id)
        {
            return DeleteAsync(id);
        }

    }
}