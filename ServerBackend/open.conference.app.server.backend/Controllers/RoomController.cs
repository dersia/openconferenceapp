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
    public class RoomController : BaseTableController<Room>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DevopenspaceContext context = new DevopenspaceContext();
            DomainManager = new EntityDomainManager<Room>(context, Request, true);
        }

        public IQueryable<Room> GetAllRoom()
        {
            return Query();
        }

        public SingleResult<Room> GetRoom(string id)
        {
            return Lookup(id);
        }

        [EmployeeAuthorize]
        public Task<Room> PatchRoom(string id, Delta<Room> patch)
        {
            return UpdateAsync(id, patch);
        }

        [EmployeeAuthorize]
        public async Task<IHttpActionResult> PostRoom(Room item)
        {
            Room current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [EmployeeAuthorize]
        [HttpPut]
        [ResponseType(typeof(List<Room>))]
        public async Task<IHttpActionResult> PutNotification(List<Room> items)
        {
            await Clean();
            var newItems = new List<Room>();
            foreach (var item in items)
            {
                newItems.Add(await InsertAsync(item));
            }
            return Ok(newItems);
        }

        [EmployeeAuthorize]
        public Task DeleteRoom(string id)
        {
            return DeleteAsync(id);
        }

    }
}