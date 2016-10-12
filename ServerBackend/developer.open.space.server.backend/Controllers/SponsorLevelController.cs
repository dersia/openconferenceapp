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
    public class SponsorLevelController : BaseTableController<SponsorLevel>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DevopenspaceContext context = new DevopenspaceContext();
            DomainManager = new EntityDomainManager<SponsorLevel>(context, Request, true);
        }

        public IQueryable<SponsorLevel> GetAllSponsorLevel()
        {
            return Query();
        }

        public SingleResult<SponsorLevel> GetSponsorLevel(string id)
        {
            return Lookup(id);
        }

        [EmployeeAuthorize]
        public Task<SponsorLevel> PatchSponsorLevel(string id, Delta<SponsorLevel> patch)
        {
            return UpdateAsync(id, patch);
        }


        [EmployeeAuthorize]
        public async Task<IHttpActionResult> PostSponsorLevel(SponsorLevel item)
        {
            SponsorLevel current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [EmployeeAuthorize]
        [HttpPut]
        [ResponseType(typeof(List<SponsorLevel>))]
        public async Task<IHttpActionResult> PutNotification(List<SponsorLevel> items)
        {
            await Clean();
            var newItems = new List<SponsorLevel>();
            foreach (var item in items)
            {
                newItems.Add(await InsertAsync(item));
            }
            return Ok(newItems);
        }

        [EmployeeAuthorize]
        public Task DeleteSponsorLevel(string id)
        {
            return DeleteAsync(id);
        }

    }
}