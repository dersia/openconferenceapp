namespace open.conference.app.server.backend.Migrations
{
    using Microsoft.Azure.Mobile.Server.Tables;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<open.conference.app.server.backend.Models.DevopenspaceContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SqlClient", new EntityTableSqlGenerator());
            ContextKey = "open.conference.app.server.backend.Models.DevopenspaceContext";
        }

        protected override void Seed(open.conference.app.server.backend.Models.DevopenspaceContext context)
        {
        }
    }
}
