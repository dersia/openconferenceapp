using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace open.conference.app.server.backend.Models
{
    public class DevopenspaceContextInitializer : DropCreateDatabaseIfModelChanges<DevopenspaceContext>
    {
        protected override void Seed(DevopenspaceContext context)
        {
            //Seed Data Here
        }
    }
}