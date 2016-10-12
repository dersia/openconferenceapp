using developer.open.space.server.backend.DataObjects;
using developer.open.space.server.backend.Identity;
using developer.open.space.server.backend.Models;
using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.OData;

namespace developer.open.space.server.backend.Controllers
{
    public class ApplicationDataController : BaseTableController<ApplicationData>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            
            DomainManager = new EntityDomainManager<ApplicationData>(Context, Request, true);
        }

        public IQueryable<ApplicationData> GetAllNotification()
        {
            return Query();
        }

        public SingleResult<ApplicationData> GetNotification(string id)
        {
            return Lookup(id);
        }

        [EmployeeAuthorize]
        public Task<ApplicationData> PatchNotification(string id, Delta<ApplicationData> patch)
        {
            return UpdateAsync(id, patch);
        }

        [EmployeeAuthorize]
        public async Task<IHttpActionResult> PostNotification(ApplicationData item)
        {
            ApplicationData current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [EmployeeAuthorize]
        [HttpPut]
        [ResponseType(typeof(List<ApplicationData>))]
        public async Task<IHttpActionResult> PutNotification(List<ApplicationData> items)
        {
            await Clean();
            var newItems = new List<ApplicationData>();
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