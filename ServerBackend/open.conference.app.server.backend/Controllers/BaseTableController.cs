using open.conference.app.server.backend.Models;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace open.conference.app.server.backend.Controllers
{
    public abstract class BaseTableController<T> :TableController<T> where T: class, ITableData
    {
        protected DevopenspaceContext Context { get; private set; } 
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            controllerContext.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            base.Initialize(controllerContext);
            Context = new DevopenspaceContext();
        }

        protected async Task Clean()
        {
            var all = await Query().ToListAsync();
            all.ForEach(item => item.Deleted = true);
            await Context.SaveChangesAsync();
        }

    }
}