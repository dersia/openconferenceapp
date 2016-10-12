using developer.open.space.server.backend.DataObjects;
using developer.open.space.server.backend.Helpers;
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
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FavoriteController : BaseTableController<Favorite>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DevopenspaceContext context = new DevopenspaceContext();
            DomainManager = new EntityDomainManager<Favorite>(context, Request, true);

        }

        public IQueryable<Favorite> GetAllFavorite()
        {
            var items = Query();
            var email = EmailHelper.GetAuthenticatedUserEmail(RequestContext);

            var final = items.Where(favorite => favorite.UserId == email);

            return final;
        }

        [Authorize]
        public SingleResult<Favorite> GetFavorite(string id)
        {
            return Lookup(id);
        }

        [Authorize]
        public Task<Favorite> PatchFavorite(string id, Delta<Favorite> patch)
        {
            return UpdateAsync(id, patch);
        }

        [Authorize]
        public async Task<IHttpActionResult> PostFavorite(Favorite item)
        {
            item.UserId = EmailHelper.GetAuthenticatedUserEmail(RequestContext);

            var current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [Authorize]
        public Task DeleteFavorite(string id)
        {
            return DeleteAsync(id);
        }

    }
}