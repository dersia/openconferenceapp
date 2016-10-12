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
    public class CategoryController : BaseTableController<Category>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DevopenspaceContext context = new DevopenspaceContext();
            DomainManager = new EntityDomainManager<Category>(context, Request, true);
        }

        public IQueryable<Category> GetAllCategory()
        {
            return Query();
        }

        public SingleResult<Category> GetCategory(string id)
        {
            return Lookup(id);
        }

        [EmployeeAuthorize]
        public Task<Category> PatchCategory(string id, Delta<Category> patch)
        {
            return UpdateAsync(id, patch);
        }

        [EmployeeAuthorize]
        public async Task<IHttpActionResult> PostCategory(Category item)
        {
            Category current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [EmployeeAuthorize]
        [HttpPut]
        [ResponseType(typeof(List<Category>))]
        public async Task<IHttpActionResult> PutNotification(List<Category> items)
        {
            await Clean();
            var newItems = new List<Category>();
            foreach (var item in items)
            {
                newItems.Add(await InsertAsync(item));
            }
            return Ok(newItems);
        }

        [EmployeeAuthorize]
        public Task DeleteCategory(string id)
        {
            return DeleteAsync(id);
        }

    }
}