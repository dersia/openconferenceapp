using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace developer.open.space.server.backend.Models
{
    public class DevopenspaceContextInitializer : DropCreateDatabaseIfModelChanges<DevopenspaceContext>
    {
        protected override void Seed(DevopenspaceContext context)
        {
            //Seed Data Here
        }
    }
}